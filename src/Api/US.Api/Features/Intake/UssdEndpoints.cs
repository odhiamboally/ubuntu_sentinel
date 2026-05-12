using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using US.Api.Features.Realtime;
using US.Api.Features.Reports;
using US.SharedKernel.Contracts.Intake;
using US.SharedKernel.Contracts.Reports;
using US.SharedKernel.Enums;
using US.SharedKernel.Inference;

namespace US.Api.Features.Intake;

public static class UssdEndpoints
{
    public static IEndpointRouteBuilder MapUssdEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/intake/ussd")
            .WithTags("USSD Intake");

        group.MapPost("/", HandleSessionAsync);

        return endpoints;
    }

    private static async Task<IResult> HandleSessionAsync(
        [FromBody] UssdSessionRequest request,
        UssdSessionStore sessions,
        IReportStore reports,
        IHubContext<ReportHub> hubContext,
        IssueTypeInferenceService issueTypeInference,
        CancellationToken cancellationToken)
    {
        var steps = request.Text.Split('*', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        var sessionId = string.IsNullOrWhiteSpace(request.SessionId) ? Guid.CreateVersion7().ToString("N") : request.SessionId;

        if (steps.Length == 0)
        {
            var languageCode = NormalizeLanguage(request.LanguageCode, request.RegionCode);
            sessions.Set(sessionId, new UssdSubmissionDraft
            {
                RegionCode = NormalizeRegion(request.RegionCode),
                LanguageCode = languageCode
            });

            return Results.Ok(Continue(sessionId, Prompt(languageCode, "main-menu")));
        }

        var flow = steps[0] == "2" ? "status" : "report";

        if (flow == "status")
        {
            var languageCode = sessions.Get(sessionId).LanguageCode;
            return Results.Ok(End(sessionId, Prompt(languageCode, "status-next")));
        }

        if (steps.Length == 1)
        {
            var existing = sessions.Get(sessionId);
            var draft = existing with
            {
                Flow = flow,
                RegionCode = NormalizeRegion(existing.RegionCode),
                LanguageCode = NormalizeLanguage(existing.LanguageCode, existing.RegionCode)
            };
            sessions.Set(sessionId, draft);
            return Results.Ok(Continue(sessionId, Prompt(draft.LanguageCode, "region")));
        }

        if (steps.Length == 2)
        {
            var regionCode = RegionFromStep(steps[1]);
            var existing = sessions.Get(sessionId);
            var draft = sessions.Get(sessionId) with
            {
                RegionCode = regionCode,
                LanguageCode = NormalizeLanguage(existing.LanguageCode, regionCode)
            };
            sessions.Set(sessionId, draft);
            return Results.Ok(Continue(sessionId, Prompt(draft.LanguageCode, "location")));
        }

        if (steps.Length == 3)
        {
            var draft = sessions.Get(sessionId) with { LocationName = steps[2] };
            sessions.Set(sessionId, draft);
            return Results.Ok(Continue(sessionId, Prompt(draft.LanguageCode, "issue-type")));
        }

        if (steps.Length == 4)
        {
            var draft = sessions.Get(sessionId) with { IssueTypeCode = steps[3] };
            sessions.Set(sessionId, draft);
            return Results.Ok(Continue(sessionId, Prompt(draft.LanguageCode, "urgency")));
        }

        if (steps.Length == 5)
        {
            var draft = sessions.Get(sessionId) with { UrgencyCode = steps[4] };
            sessions.Set(sessionId, draft);
            return Results.Ok(Continue(sessionId, Prompt(draft.LanguageCode, "flags")));
        }

        if (steps.Length == 6)
        {
            var draft = sessions.Get(sessionId) with { FlagsCode = steps[5] };
            sessions.Set(sessionId, draft);
            return Results.Ok(Continue(sessionId, Prompt(draft.LanguageCode, "description")));
        }

        var current = sessions.Get(sessionId);
        var description = steps[^1];
        var flags = FlagsFrom(current.FlagsCode);
        var inferredIssue = issueTypeInference.Infer(description);
        var submitRequest = new SubmitReportRequest
        {
            RegionCode = current.RegionCode,
            LanguageCode = current.LanguageCode,
            LocationName = current.LocationName,
            Description = description,
            IssueTypeHint = IssueTypeFrom(current.Flow, current.IssueTypeCode),
            UrgencyHint = UrgencyFrom(current.UrgencyCode),
            WomenLed = flags.WomenLed,
            YouthLed = flags.YouthLed,
            IsSensitive = flags.IsSensitive
        };

        var report = await reports.CreateAsync(submitRequest, cancellationToken);
        await hubContext.Clients.All.SendAsync("ReportSubmitted", report, cancellationToken);
        sessions.Remove(sessionId);

        return Results.Ok(End(
            sessionId,
            Prompt(current.LanguageCode, "received", category: FormatIssue(current.LanguageCode, report.IssueType)),
            report.Id));
    }

    private static string Prompt(string languageCode, string key, string? value = null, string? category = null, decimal confidence = 0)
    {
        var isFrench = string.Equals(languageCode, "fr", StringComparison.OrdinalIgnoreCase);

        return key switch
        {
            "location" => isFrench ? "Entrez le village, la ville ou le point de repere" : "Enter village, town, or landmark",
            "issue-type" => isFrench
                ? """
                Type de probleme
                1. Promesse non tenue
                2. Conflit de ressources
                3. Defaillance de gouvernance
                4. Probleme de securite
                5. Manque de service
                6. Prejudice environnemental
                7. Mediation reussie
                8. Besoin de retablissement
                9. Resilience communautaire
                """
                : """
                Issue type
                1. Broken promise
                2. Resource conflict
                3. Governance failure
                4. Security concern
                5. Service gap
                6. Environmental harm
                7. Mediation success
                8. Recovery need
                9. Community resilience
                """,
            "urgency" => isFrench
                ? """
                Urgence
                1. Faible
                2. Moyenne
                3. Elevee
                4. Critique
                """
                : """
                Urgency
                1. Low
                2. Medium
                3. High
                4. Critical
                """,
            "flags" => isFrench
                ? """
                Indicateurs optionnels
                0. Aucun
                1. Dirige par des femmes
                2. Dirige par des jeunes
                3. Sensible/anonymiser
                Entrez plusieurs chiffres ensemble, ex: 13
                """
                : """
                Optional flags
                0. None
                1. Women-led
                2. Youth-led
                3. Sensitive/anonymize
                Enter multiple digits together, e.g. 13
                """,
            "description" => isFrench ? "Decrivez ce qui s'est passe avec les mots de la communaute" : "Describe what happened in community words",
            "received" => isFrench
                ? $"Rapport recu pour validation. Categorie: {category}."
                : $"Report received for validation. Category: {category}.",
            "main-menu" => isFrench
                ? """
                Ubuntu Sentinel
                1. Soumettre un rapport communautaire
                2. Verifier le statut d'un rapport
                """
                : """
                Ubuntu Sentinel
                1. Submit a community report
                2. Check report status
                """,
            "region" => isFrench
                ? """
                Selectionnez la region
                1. Sahel
                2. Est de la RDC
                3. Soudan
                4. Mozambique
                """
                : """
                Select region
                1. Sahel
                2. Eastern DRC
                3. Sudan
                4. Mozambique
                """,
            "status-next" => isFrench
                ? "La recherche de statut par identifiant sera disponible dans la prochaine tranche. Vos rapports communautaires restent soumis a validation."
                : "Status lookup is ready for reporter IDs in the next slice. Your community reports remain validation-gated.",
            _ => key
        };
    }

    private static UssdSessionResponse Continue(string sessionId, string message)
    {
        return new UssdSessionResponse
        {
            SessionId = sessionId,
            Message = $"CON {message}",
            IsTerminal = false
        };
    }

    private static UssdSessionResponse End(string sessionId, string message, Guid? reportId = null)
    {
        return new UssdSessionResponse
        {
            SessionId = sessionId,
            Message = $"END {message}",
            IsTerminal = true,
            ReportId = reportId,
            ReportStatus = reportId.HasValue ? "Pending validation" : null
        };
    }

    private static string RegionFromStep(string value) => value switch
    {
        "2" => "drc",
        "3" => "sudan",
        "4" => "mozambique",
        _ => "sahel"
    };

    private static string LanguageForRegion(string regionCode) => regionCode switch
    {
        "sahel" or "drc" => "fr",
        _ => "en"
    };

    private static string NormalizeLanguage(string? languageCode, string? regionCode)
    {
        if (string.Equals(languageCode, "fr", StringComparison.OrdinalIgnoreCase))
        {
            return "fr";
        }

        if (string.Equals(languageCode, "en", StringComparison.OrdinalIgnoreCase))
        {
            return "en";
        }

        return LanguageForRegion(NormalizeRegion(regionCode));
    }

    private static string NormalizeRegion(string? regionCode)
    {
        return regionCode is not null
            && (string.Equals(regionCode, "drc", StringComparison.OrdinalIgnoreCase)
                || string.Equals(regionCode, "sudan", StringComparison.OrdinalIgnoreCase)
                || string.Equals(regionCode, "mozambique", StringComparison.OrdinalIgnoreCase))
            ? regionCode.ToLowerInvariant()
            : "sahel";
    }

    private static IssueType IssueTypeFrom(string flow, string value)
    {
        return value switch
        {
            "2" => IssueType.ResourceConflict,
            "3" => IssueType.GovernanceFailure,
            "4" => IssueType.SecurityConcern,
            "5" => IssueType.ServiceGap,
            "6" => IssueType.EnvironmentalHarm,
            "7" => IssueType.MediationSuccess,
            "8" => IssueType.RecoveryNeed,
            "9" => IssueType.CommunityResilience,
            _ => IssueType.BrokenPromise
        };
    }

    private static UrgencyLevel UrgencyFrom(string value) => value switch
    {
        "1" => UrgencyLevel.Low,
        "3" => UrgencyLevel.High,
        "4" => UrgencyLevel.Critical,
        _ => UrgencyLevel.Medium
    };

    private static (bool WomenLed, bool YouthLed, bool IsSensitive) FlagsFrom(string value)
    {
        var normalized = value.Trim();

        return (
            normalized.Contains('1'),
            normalized.Contains('2'),
            normalized.Contains('3'));
    }

    private static string FormatIssue(string languageCode, IssueType issueType)
    {
        var isFrench = string.Equals(languageCode, "fr", StringComparison.OrdinalIgnoreCase);

        return isFrench
            ? issueType switch
            {
                IssueType.BrokenPromise => "Promesse non tenue",
                IssueType.ResourceConflict => "Conflit de ressources",
                IssueType.GovernanceFailure => "Defaillance de gouvernance",
                IssueType.SecurityConcern => "Probleme de securite",
                IssueType.ServiceGap => "Manque de service",
                IssueType.EnvironmentalHarm => "Prejudice environnemental",
                IssueType.MediationSuccess => "Mediation reussie",
                IssueType.RecoveryNeed => "Besoin de retablissement",
                IssueType.CommunityResilience => "Resilience communautaire",
                _ => "Autre"
            }
            : issueType switch
            {
                IssueType.BrokenPromise => "Broken promise",
                IssueType.ResourceConflict => "Resource conflict",
                IssueType.GovernanceFailure => "Governance failure",
                IssueType.SecurityConcern => "Security concern",
                IssueType.ServiceGap => "Service gap",
                IssueType.EnvironmentalHarm => "Environmental harm",
                IssueType.MediationSuccess => "Mediation success",
                IssueType.RecoveryNeed => "Recovery need",
                IssueType.CommunityResilience => "Community resilience",
                _ => "Other"
            };
    }
}
