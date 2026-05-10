using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using US.Api.Features.Realtime;
using US.Api.Features.Reports;
using US.SharedKernel.Contracts.Intake;
using US.SharedKernel.Contracts.Reports;
using US.SharedKernel.Enums;

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
        CancellationToken cancellationToken)
    {
        var steps = request.Text.Split('*', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        var sessionId = string.IsNullOrWhiteSpace(request.SessionId) ? Guid.NewGuid().ToString("N") : request.SessionId;

        if (steps.Length == 0)
        {
            sessions.Set(sessionId, new UssdSubmissionDraft());
            return Results.Ok(Continue(sessionId, """
            Ubuntu Sentinel
            1. Report broken promise or harm
            2. Report peace/resilience story
            3. Check report status
            """));
        }

        var flow = steps[0] switch
        {
            "2" => "resilience",
            "3" => "status",
            _ => "incident"
        };

        if (flow == "status")
        {
            return Results.Ok(End(sessionId, "Status lookup is ready for reporter IDs in the next slice. Your community reports remain validation-gated."));
        }

        if (steps.Length == 1)
        {
            var draft = sessions.Get(sessionId) with { Flow = flow };
            sessions.Set(sessionId, draft);
            return Results.Ok(Continue(sessionId, """
            Select region
            1. Sahel
            2. Eastern DRC
            3. Sudan
            4. Mozambique
            """));
        }

        if (steps.Length == 2)
        {
            var draft = sessions.Get(sessionId) with { RegionCode = RegionFromStep(steps[1]) };
            sessions.Set(sessionId, draft);
            return Results.Ok(Continue(sessionId, "Enter village, town, or landmark"));
        }

        if (steps.Length == 3)
        {
            var draft = sessions.Get(sessionId) with { LocationName = steps[2] };
            sessions.Set(sessionId, draft);
            return Results.Ok(Continue(sessionId, flow == "resilience"
                ? """
                Peace story type
                1. Women-led mediation
                2. Youth early warning
                3. Shared resource agreement
                4. Other resilience
                """
                : """
                Issue type
                1. Broken promise
                2. Resource conflict
                3. Security concern
                4. Environmental harm
                """));
        }

        if (steps.Length == 4)
        {
            var draft = sessions.Get(sessionId) with { IssueTypeCode = steps[3] };
            sessions.Set(sessionId, draft);
            return Results.Ok(Continue(sessionId, "Describe what happened in community words"));
        }

        var current = sessions.Get(sessionId);
        var description = steps[^1];
        var submitRequest = new SubmitReportRequest
        {
            RegionCode = current.RegionCode,
            LanguageCode = current.LanguageCode,
            LocationName = current.LocationName,
            Description = description,
            IssueTypeHint = IssueTypeFrom(current.Flow, current.IssueTypeCode),
            WomenLed = current.Flow == "resilience" && current.IssueTypeCode == "1",
            YouthLed = current.Flow == "resilience" && current.IssueTypeCode == "2",
            IsSensitive = current.Flow == "incident" && current.IssueTypeCode == "3"
        };

        var report = await reports.CreateAsync(submitRequest, cancellationToken);
        await hubContext.Clients.All.SendAsync("ReportSubmitted", report, cancellationToken);
        sessions.Remove(sessionId);

        return Results.Ok(End(sessionId, $"Report received for human validation. Ref: {report.Id.ToString()[..8]}", report.Id));
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

    private static IssueType IssueTypeFrom(string flow, string value)
    {
        if (flow == "resilience")
        {
            return value switch
            {
                "1" => IssueType.MediationSuccess,
                "2" => IssueType.CommunityResilience,
                "3" => IssueType.ResourceConflict,
                _ => IssueType.CommunityResilience
            };
        }

        return value switch
        {
            "2" => IssueType.ResourceConflict,
            "3" => IssueType.SecurityConcern,
            "4" => IssueType.EnvironmentalHarm,
            _ => IssueType.BrokenPromise
        };
    }
}
