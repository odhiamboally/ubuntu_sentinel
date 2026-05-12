using System.Globalization;
using US.SharedKernel.Contracts.Reports;
using US.SharedKernel.Enums;

namespace US.Api.Features.Accountability;

public sealed class DeterministicAccountabilityPipeline(IPolicyComparisonService policyComparison) : IAccountabilityPipeline
{
    public Task<ReportPipelineResultDto> AnalyzeAsync(ReportDto report, CancellationToken cancellationToken)
    {
        var policyMatch = policyComparison.FindBestMatch(report);
        var householdCount = InferHouseholds(report.Description);
        var daysOfHarm = InferDays(report.Description);
        var estimatedCost = householdCount * 2 * daysOfHarm;
        var hasCommitment = policyMatch is not null
            || !string.IsNullOrWhiteSpace(report.ReferencedCommitment)
            || report.Description.Contains("promised", StringComparison.OrdinalIgnoreCase)
            || report.Description.Contains("pledged", StringComparison.OrdinalIgnoreCase)
            || report.Description.Contains("agreement", StringComparison.OrdinalIgnoreCase)
            || report.Description.Contains("cda", StringComparison.OrdinalIgnoreCase);

        var isFrench = IsFrench(report.LanguageCode);
        var summary = isFrench
            ? $"{report.Location.Name} a soumis un rapport de type {FormatIssueType(report.IssueType, report.LanguageCode)} avec une urgence {FormatUrgency(report.Urgency, report.LanguageCode)}. Le rapport attend une validation humaine avant toute escalade."
            : $"{report.Location.Name} submitted a {FormatIssueType(report.IssueType, report.LanguageCode)} report with {report.Urgency.ToString().ToLowerInvariant()} urgency. The report is pending human validation before escalation.";
        var gapAnalysis = policyMatch is not null
            ? policyMatch.Gap
            : hasCommitment
                ? isFrench
                    ? $"Le signalement \"{TrimForDisplay(report.Description)}\" semble decrire un ecart entre un engagement declare et la realite observee par la communaute. Un validateur humain doit confirmer la clause exacte, l'acteur responsable, l'echeance et les preuves disponibles."
                    : $"The claim \"{TrimForDisplay(report.Description)}\" appears to describe a gap between a stated commitment and the observed community reality. A human validator should confirm the exact clause, responsible actor, deadline, and current evidence."
            : isFrench
                ? $"Le signalement \"{TrimForDisplay(report.Description)}\" n'identifie pas encore clairement un engagement verifiable. Un validateur doit demander l'accord, la politique, la promesse, l'obligation de service public ou le cadre de droits qui fonde la demande de responsabilite."
                : $"The claim \"{TrimForDisplay(report.Description)}\" does not clearly identify a verifiable commitment yet. A validator should request the agreement, policy, promise, public service obligation, or rights framework that anchors the accountability claim.";

        var reparativeProposal = BuildReparativeProposal(report, householdCount, daysOfHarm, estimatedCost);
        var sensitiveHandlingCleared = report.IsSensitive
            && report.ValidationChecks.ConsentConfirmed
            && report.ValidationChecks.ReporterSafetyChecked;
        var isRejected = report.Status == ReportStatus.Rejected;

        return Task.FromResult(new ReportPipelineResultDto
        {
            ReportId = report.Id,
            IssueType = report.IssueType,
            Urgency = report.Urgency,
            Confidence = hasCommitment ? 0.74m : 0.58m,
            Summary = summary,
            GapAnalysis = gapAnalysis,
            ReparativeProposal = reparativeProposal,
            AccountabilityBriefMarkdown = BuildBrief(report, summary, gapAnalysis, reparativeProposal, policyMatch),
            PipelineMode = "Deterministic fallback",
            PolicyMatch = policyMatch,
            Steps =
            [
                new PipelineStepDto
                {
                    Name = "Agent 1 - Evidence Structuring",
                    Purpose = isFrench ? "Transformer le temoignage communautaire en fiche d'intelligence structuree." : "Convert raw community testimony into a structured intelligence record.",
                    Output = isFrench
                        ? $"Classe comme {FormatIssueType(report.IssueType, report.LanguageCode)} avec une urgence {FormatUrgency(report.Urgency, report.LanguageCode)} et une confiance de localisation {FormatConfidence(report.Location.Confidence, report.LanguageCode)}."
                        : $"Classified as {FormatIssueType(report.IssueType, report.LanguageCode)} with {report.Urgency.ToString().ToLowerInvariant()} urgency and {report.Location.Confidence.ToString().ToLowerInvariant()} location confidence.",
                    Confidence = 0.72m
                },
                new PipelineStepDto
                {
                    Name = "Agent 2 - Policy RAG Comparison",
                    Purpose = isFrench ? "Comparer le rapport aux clauses, engagements et cadres regionaux sources." : "Compare the report against real sourced clauses, commitments, and regional frameworks.",
                    Output = policyMatch is not null
                        ? isFrench
                            ? $"Correspondance avec {policyMatch.DocumentTitle}, {policyMatch.ArticleReference} ({policyMatch.Similarity:0.00} similarite). Ecart : {policyMatch.Gap}"
                            : $"Matched {policyMatch.DocumentTitle}, {policyMatch.ArticleReference} ({policyMatch.Similarity:0.00} similarity). Gap: {policyMatch.Gap}"
                        : hasCommitment
                            ? isFrench ? "Reference potentielle a un engagement detectee ; le validateur doit joindre le document exact." : "Potential commitment reference detected; human reviewer should attach the exact document."
                        : isFrench ? "Aucune reference claire a un engagement detectee ; le rapport necessite des preuves de suivi avant escalation." : "No clear commitment reference detected; report needs follow-up evidence before escalation.",
                    Confidence = policyMatch?.Similarity ?? (hasCommitment ? 0.70m : 0.48m)
                },
                new PipelineStepDto
                {
                    Name = "Agent 3 - Safety and Ethics Review",
                    Purpose = isFrench ? "Verifier la sensibilite, le consentement et les risques d'exposition du declarant ou de la communaute." : "Check sensitivity, consent requirements, and whether escalation could expose the reporter or community.",
                    Output = isRejected
                        ? isFrench ? "Validation rejetee. Les controles de validation ne sont pas consideres comme valides et aucune escalation ne doit etre engagee." : "Validation rejected. The validation checks are not treated as cleared, and no escalation should proceed."
                        : sensitiveHandlingCleared
                        ? isFrench ? "Traitement sensible verifie. Le consentement et la securite du declarant ont ete confirmes par le validateur ; les details peuvent etre utilises dans les vues appropriees avec prudence." : "Sensitive handling cleared. The validator confirmed consent and reporter safety; details may be used in appropriate views with care."
                        : report.IsSensitive
                        ? isFrench ? "Traitement sensible requis. Le validateur doit confirmer le consentement et l'anonymisation avant escalation." : "Sensitive handling required. Validator must confirm consent and anonymization before escalation."
                        : isFrench ? "Aucun indicateur sensible selectionne. Le validateur confirme quand meme le consentement, la fiabilite du lieu, la qualite des preuves et la securite du declarant." : "No sensitivity flag was selected. Validator still confirms consent, location confidence, evidence quality, and reporter safety.",
                    Confidence = isRejected ? 0.90m : sensitiveHandlingCleared ? 0.82m : report.IsSensitive ? 0.69m : 0.76m
                },
                new PipelineStepDto
                {
                    Name = "Agent 4 - Accountability Brief Generation",
                    Purpose = isFrench ? "Generer un brief de responsabilite soumis a validation pour les defenseurs et partenaires OSF." : "Generate a validation-gated accountability brief for advocates and OSF partners.",
                    Output = isRejected
                        ? isFrench ? "Brief de responsabilite bloque. Le rapport a ete rejete pendant la validation ; cette sortie reste interne et ne doit pas etre escaladee." : "Accountability brief held. The report was rejected during validation, so this output remains internal and should not be escalated."
                        : isFrench ? $"Brief redige avec analyse d'ecart et proposition reparatrice : {reparativeProposal}" : $"Drafted brief with gap analysis and reparative proposal: {reparativeProposal}",
                    Confidence = isRejected ? 0.90m : 0.76m
                }
            ],
            Flags = BuildFlags(report, hasCommitment),
            Citations = policyMatch is not null
                ? [$"{policyMatch.DocumentTitle}, {policyMatch.ArticleReference} ({policyMatch.Source}): {policyMatch.Commitment}"]
                : hasCommitment ? ["Seed CDA demo clause: Section 4.2 - community water access commitment"] : []
        });
    }

    private static string BuildBrief(ReportDto report, string summary, string gapAnalysis, string reparativeProposal, PolicyMatchDto? policyMatch)
    {
        if (report.Status == ReportStatus.Rejected)
        {
            return BuildRejectedBrief(report, gapAnalysis, policyMatch);
        }

        if (report.Status == ReportStatus.NeedsFollowUp)
        {
            return BuildFollowUpBrief(report, summary, gapAnalysis, policyMatch);
        }

        if (IsFrench(report.LanguageCode))
        {
            return $"""
            # Brief de responsabilite : {report.Location.Name}

            **Genere** : {DateTimeOffset.UtcNow:O}  
            **Communaute** : {report.Location.Name}  
            **ID du rapport** : {report.Id}  
            **Statut de validation humaine** : {report.Status}
            **Notes du validateur** : {FormatValidatorNotes(report)}

            ## 1. Resume executif
            {summary}

            ## 2. Preuves communautaires
            - **Source** : Rapport soumis par la communaute{(report.IsSensitive ? " (sensible ; les identifiants doivent etre anonymises)" : "")}
            - **Date du rapport** : {report.SubmittedAt:yyyy-MM-dd}
            - **Signalement cle** : {report.Description}
            - **Score de confiance** : {report.TrustScore.Overall:0.00}/1.0 ({report.TrustScore.InternalConsistency})

            ## 3. Reference politique ou accord
            - **Document** : {policyMatch?.DocumentTitle ?? "Document a joindre"}
            - **Clause pertinente** : {(policyMatch is not null ? $"{policyMatch.ArticleReference}: {policyMatch.Commitment}" : "Engagement source a confirmer par validation humaine")}
            - **Source** : {policyMatch?.SourceUrl ?? "URL source a confirmer"}
            - **Ecart** : {gapAnalysis}

            ## 4. Evaluation du prejudice
            - **Personnes affectees** : A confirmer par validation communautaire
            - **Estimation economique** : Estimation de demonstration seulement ; validation communautaire requise
            - **Prejudice non economique** : Perte de dignite, erosion de la confiance et affaiblissement des voies pacifiques de responsabilite.

            ## 5. Proposition reparatrice
            - **Action immediate** : {reparativeProposal}
            - **Validation communautaire** : Requise avant escalation

            ## 6. Prochaines etapes recommandees
            1. Assigner un validateur de confiance pour confirmer les details et le consentement.
            2. Comparer le rapport a l'accord, la politique ou l'engagement public pertinent.
            3. Preparer l'escalade uniquement apres l'examen de securite communautaire.

            ## 7. Principe Ubuntu
            La reparation doit restaurer la dignite, la solidarite et le bien-etre collectif, pas seulement regler une transaction.

            ---
            *Genere avec assistance IA. Toute affirmation requiert une validation humaine avant escalation institutionnelle.*
            """;
        }

        return $"""
        # Accountability Brief: {report.Location.Name}

        **Generated**: {DateTimeOffset.UtcNow:O}  
        **Community**: {report.Location.Name}  
            **Report ID**: {report.Id}  
            **Human validation status**: {report.Status}
            **Validator notes**: {FormatValidatorNotes(report)}

        ## 1. Executive Summary
        {summary}

        ## 2. Community Evidence
        - **Source**: Community-submitted report{(report.IsSensitive ? " (sensitive; identifiers should be anonymized)" : "")}
        - **Date Reported**: {report.SubmittedAt:yyyy-MM-dd}
        - **Key Claim**: {report.Description}
        - **Trust Score**: {report.TrustScore.Overall:0.00}/1.0 ({report.TrustScore.InternalConsistency})

        ## 3. Policy/Agreement Reference
        - **Document**: {policyMatch?.DocumentTitle ?? "Pending document attachment"}
        - **Relevant Clause**: {(policyMatch is not null ? $"{policyMatch.ArticleReference}: {policyMatch.Commitment}" : "Demo seed commitment, pending human verification")}
        - **Source**: {policyMatch?.SourceUrl ?? "Pending source URL"}
        - **Contradiction**: {gapAnalysis}

        ## 4. Harm Assessment
        - **Affected**: Pending community validation
        - **Economic Estimate**: Demo estimate only; community validation required
        - **Non-Economic Harm**: Eroded trust, dignity loss, and weakened confidence in peaceful accountability channels.

        ## 5. Reparative Proposal
        - **Immediate Action**: {reparativeProposal}
        - **Community Validation**: Required before escalation

        ## 6. Recommended Next Steps
        1. Assign a trusted validator to confirm details and consent.
        2. Match the report against the relevant agreement, policy, or public commitment.
        3. Prepare escalation only after community safety review.

        ## 7. Ubuntu Principle
        Repair should restore dignity, solidarity, and collective wellbeing, not only settle a transaction.

        ---
        *Generated with AI assistance. All claims require human validation before institutional escalation.*
        """;
    }

    private static string BuildRejectedBrief(ReportDto report, string gapAnalysis, PolicyMatchDto? policyMatch)
    {
        if (IsFrench(report.LanguageCode))
        {
            return $"""
            # Dossier interne de rejet : {report.Location.Name}

            **Genere** : {DateTimeOffset.UtcNow:O}  
            **Communaute** : {report.Location.Name}  
            **ID du rapport** : {report.Id}  
            **Statut** : Rejete  
            **Notes du validateur** : {FormatValidatorNotes(report)}

            ## 1. Decision de validation
            Ce rapport a ete rejete pendant la validation. Il ne doit pas etre utilise pour une escalation institutionnelle, un plaidoyer externe ou une revendication publique sans nouvelle soumission ou revalidation.

            ## 2. Signalement conserve en interne
            - **Type** : {FormatIssueType(report.IssueType, report.LanguageCode)}
            - **Urgence indiquee** : {FormatUrgency(report.Urgency, report.LanguageCode)}
            - **Signalement cle** : {report.Description}

            ## 3. Reference source non validee
            - **Document apparie** : {policyMatch?.DocumentTitle ?? "Aucun document source confirme"}
            - **Clause** : {(policyMatch is not null ? $"{policyMatch.ArticleReference}: {policyMatch.Commitment}" : "Non confirmee")}
            - **Analyse non escaladee** : {gapAnalysis}

            ## 4. Raison operationnelle
            Les controles de consentement, de securite, de fiabilite du lieu et/ou de qualite des preuves ne sont pas traites comme acquis pour un rapport rejete.

            ## 5. Prochaines etapes
            1. Conserver ce dossier comme trace interne.
            2. Informer le rapporteur, si cela est sur, qu'une nouvelle soumission peut etre necessaire avec plus de preuves ou un consentement clair.
            3. Ne pas transmettre ce rapport aux partenaires externes comme preuve validee.

            ## 6. Principe Ubuntu
            Le rejet doit proteger la dignite, la securite et la confiance communautaire, pas fermer la porte a une soumission plus sure et mieux etayee.

            ---
            *Dossier interne genere avec assistance IA. Rapport rejete ; aucune escalation autorisee.*
            """;
        }

        return $"""
        # Internal Rejection Record: {report.Location.Name}

        **Generated**: {DateTimeOffset.UtcNow:O}  
        **Community**: {report.Location.Name}  
        **Report ID**: {report.Id}  
        **Status**: Rejected  
        **Validator notes**: {FormatValidatorNotes(report)}

        ## 1. Validation Decision
        This report was rejected during validation. It should not be used for institutional escalation, external advocacy, or public claims without resubmission or revalidation.

        ## 2. Internal Report Record
        - **Type**: {FormatIssueType(report.IssueType, report.LanguageCode)}
        - **Stated urgency**: {report.Urgency}
        - **Key claim**: {report.Description}

        ## 3. Unvalidated Source Match
        - **Matched document**: {policyMatch?.DocumentTitle ?? "No confirmed source document"}
        - **Clause**: {(policyMatch is not null ? $"{policyMatch.ArticleReference}: {policyMatch.Commitment}" : "Not confirmed")}
        - **Non-escalated analysis**: {gapAnalysis}

        ## 4. Operational Reason
        Consent, reporter safety, location confidence, and/or evidence quality checks are not treated as cleared for a rejected report.

        ## 5. Next Steps
        1. Keep this as an internal audit record.
        2. Notify the reporter, where safe, that resubmission may be needed with clearer evidence or consent.
        3. Do not transmit this report to external partners as validated evidence.

        ## 6. Ubuntu Principle
        Rejection should protect dignity, safety, and community trust, not close the door to a safer and better-supported submission.

        ---
        *Internal record generated with AI assistance. Report rejected; no escalation authorized.*
        """;
    }

    private static string BuildFollowUpBrief(ReportDto report, string summary, string gapAnalysis, PolicyMatchDto? policyMatch)
    {
        if (IsFrench(report.LanguageCode))
        {
            return $"""
            # Brief de suivi : {report.Location.Name}

            **Genere** : {DateTimeOffset.UtcNow:O}  
            **Communaute** : {report.Location.Name}  
            **ID du rapport** : {report.Id}  
            **Statut** : Suivi requis  
            **Notes du validateur** : {FormatValidatorNotes(report)}

            ## 1. Resume
            {summary}

            ## 2. Pourquoi un suivi est requis
            Le rapport contient un signal potentiellement pertinent, mais le validateur a demande un suivi avant toute escalation.

            ## 3. Elements a confirmer
            - Consentement et securite du rapporteur
            - Details de lieu et acteurs responsables
            - Qualite des preuves disponibles
            - Document ou engagement source exact

            ## 4. Reference provisoire
            - **Document** : {policyMatch?.DocumentTitle ?? "Document a joindre"}
            - **Clause** : {(policyMatch is not null ? $"{policyMatch.ArticleReference}: {policyMatch.Commitment}" : "A confirmer")}
            - **Ecart provisoire** : {gapAnalysis}

            ## 5. Actions de suivi
            1. Recontacter le rapporteur ou un facilitateur de confiance si cela est sur.
            2. Ajouter les pieces, temoignages ou documents manquants.
            3. Revenir a la validation avant toute escalation externe.

            ---
            *Brief de suivi genere avec assistance IA. Non pret pour escalation.*
            """;
        }

        return $"""
        # Follow-up Brief: {report.Location.Name}

        **Generated**: {DateTimeOffset.UtcNow:O}  
        **Community**: {report.Location.Name}  
        **Report ID**: {report.Id}  
        **Status**: Needs follow-up  
        **Validator notes**: {FormatValidatorNotes(report)}

        ## 1. Summary
        {summary}

        ## 2. Why Follow-up Is Required
        The report contains a potentially relevant accountability signal, but the validator requested follow-up before any escalation.

        ## 3. Items To Confirm
        - Reporter consent and safety
        - Location details and responsible actors
        - Evidence quality and supporting materials
        - Exact source document or public commitment

        ## 4. Provisional Source Reference
        - **Document**: {policyMatch?.DocumentTitle ?? "Pending document attachment"}
        - **Clause**: {(policyMatch is not null ? $"{policyMatch.ArticleReference}: {policyMatch.Commitment}" : "Pending confirmation")}
        - **Provisional gap**: {gapAnalysis}

        ## 5. Follow-up Actions
        1. Recontact the reporter or trusted facilitator where safe.
        2. Add missing evidence, testimony, or source documents.
        3. Return to validation before any external escalation.

        ---
        *Follow-up brief generated with AI assistance. Not ready for escalation.*
        """;
    }

    private static string FormatValidatorNotes(ReportDto report) =>
        string.IsNullOrWhiteSpace(report.ValidatorNotes) ? "Not provided" : report.ValidatorNotes.Trim();

    private static IReadOnlyList<string> BuildFlags(ReportDto report, bool hasCommitment)
    {
        var flags = new List<string>();

        if (!hasCommitment)
        {
            flags.Add("missing_verifiable_commitment");
        }

        if (report.Location.Confidence == ConfidenceLevel.Low)
        {
            flags.Add("low_location_confidence");
        }

        if (report.Urgency >= UrgencyLevel.High)
        {
            flags.Add("high_urgency");
        }

        if (report.IsSensitive)
        {
            flags.Add("sensitive_report");
        }

        return flags;
    }

    private static int InferHouseholds(string description)
    {
        var numbers = ExtractNumbers(description);
        return numbers.FirstOrDefault(number => number is >= 10 and <= 10_000, 200);
    }

    private static int InferDays(string description)
    {
        var normalized = description.ToLowerInvariant();

        if (normalized.Contains("six months") || normalized.Contains("6 months"))
        {
            return 180;
        }

        if (normalized.Contains("three months") || normalized.Contains("3 months"))
        {
            return 90;
        }

        return 60;
    }

    private static IReadOnlyList<int> ExtractNumbers(string value)
    {
        return value
            .Split([' ', ',', '.', ';', ':', '-', '(', ')'], StringSplitOptions.RemoveEmptyEntries)
            .Select(token => int.TryParse(token, out var number) ? number : (int?)null)
            .Where(number => number.HasValue)
            .Select(number => number!.Value)
            .ToList();
    }

    private static string TrimForDisplay(string value)
    {
        var trimmed = value.Trim();
        return trimmed.Length <= 140 ? trimmed : $"{trimmed[..137]}...";
    }

    private static string BuildReparativeProposal(ReportDto report, int householdCount, int daysOfHarm, int estimatedCost)
    {
        var isFrench = IsFrench(report.LanguageCode);
        return report.IssueType switch
        {
            IssueType.BrokenPromise => isFrench
                ? $"Verification immediate sous 14 jours, reponse publique de l'institution responsable et plan de livraison valide par la communaute. Estimation demo : {householdCount} menages x 2 $/jour x {daysOfHarm} jours = {estimatedCost.ToString("C0", CultureInfo.GetCultureInfo("en-US"))}."
                : $"Immediate verification within 14 days, public response from the responsible institution, and a community-validated delivery plan. Demo estimate: {householdCount} households x $2/day x {daysOfHarm} days = {estimatedCost.ToString("C0", CultureInfo.GetCultureInfo("en-US"))}.",
            IssueType.ServiceGap => isFrench
                ? "Audit immediat du service, confirmation des personnes affectees, reponse ecrite du fournisseur responsable et voie de retablissement ou remboursement avec delai."
                : "Immediate service audit, confirmation of who is affected, written response from the responsible service provider, and a time-bound restoration or refund pathway.",
            IssueType.EnvironmentalHarm => isFrench
                ? "Evaluation immediate de securite, mitigation temporaire, depistage sanitaire communautaire et plan de remediation valide par les representants locaux."
                : "Immediate safety assessment, temporary mitigation, community health screening, and a remediation plan validated by local representatives.",
            IssueType.SecurityConcern => isFrench
                ? "Orientation immediate vers des acteurs locaux de confiance, traitement anonymise et escalation uniquement apres consentement et examen des risques."
                : "Immediate safety referral to trusted local actors, anonymized handling, and escalation only after consent and risk review.",
            IssueType.GovernanceFailure => isFrench
                ? "Examen procedural immediat, preuve de preavis ou mandat, evaluation de l'impact sur les moyens de subsistance et voie de recours convenue avec les representants communautaires."
                : "Immediate procedural review, proof of notice or mandate, livelihood impact assessment, and a remedy pathway agreed with affected community representatives.",
            _ => isFrench
                ? "Session de validation communautaire, identification de l'acteur responsable et voie de reponse avec delai avant toute escalation institutionnelle."
                : "Community validation session, responsible actor identification, and a time-bound response pathway before institutional escalation."
        };
    }

    private static string FormatIssueType(IssueType issueType, string languageCode)
    {
        if (IsFrench(languageCode))
        {
            return issueType switch
            {
                IssueType.BrokenPromise => "promesse non tenue",
                IssueType.ResourceConflict => "conflit de ressources",
                IssueType.GovernanceFailure => "defaillance de gouvernance",
                IssueType.SecurityConcern => "probleme de securite",
                IssueType.ServiceGap => "manque de service",
                IssueType.EnvironmentalHarm => "prejudice environnemental",
                IssueType.MediationSuccess => "mediation reussie",
                IssueType.RecoveryNeed => "besoin de retablissement",
                IssueType.CommunityResilience => "resilience communautaire",
                _ => "responsabilite communautaire"
            };
        }

        return issueType switch
        {
            IssueType.BrokenPromise => "broken promise",
            IssueType.ResourceConflict => "resource conflict",
            IssueType.GovernanceFailure => "governance failure",
            IssueType.SecurityConcern => "security concern",
            IssueType.ServiceGap => "service gap",
            IssueType.EnvironmentalHarm => "environmental harm",
            IssueType.MediationSuccess => "mediation success",
            IssueType.RecoveryNeed => "recovery need",
            IssueType.CommunityResilience => "community resilience",
            _ => "community accountability"
        };
    }

    private static string FormatUrgency(UrgencyLevel urgency, string languageCode)
    {
        if (!IsFrench(languageCode))
        {
            return urgency.ToString().ToLowerInvariant();
        }

        return urgency switch
        {
            UrgencyLevel.Low => "faible",
            UrgencyLevel.Medium => "moyenne",
            UrgencyLevel.High => "elevee",
            UrgencyLevel.Critical => "critique",
            _ => urgency.ToString().ToLowerInvariant()
        };
    }

    private static string FormatConfidence(ConfidenceLevel confidence, string languageCode)
    {
        if (!IsFrench(languageCode))
        {
            return confidence.ToString().ToLowerInvariant();
        }

        return confidence switch
        {
            ConfidenceLevel.Low => "faible",
            ConfidenceLevel.Medium => "moyenne",
            ConfidenceLevel.High => "elevee",
            _ => confidence.ToString().ToLowerInvariant()
        };
    }

    private static bool IsFrench(string? languageCode) =>
        string.Equals(languageCode, "fr", StringComparison.OrdinalIgnoreCase);
}
