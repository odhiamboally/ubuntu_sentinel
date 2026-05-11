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

        var summary = $"{report.Location.Name} submitted a {FormatIssueType(report.IssueType)} report with {report.Urgency.ToString().ToLowerInvariant()} urgency. The report is pending human validation before escalation.";
        var gapAnalysis = policyMatch is not null
            ? policyMatch.Gap
            : hasCommitment
                ? "The report appears to describe a gap between a stated commitment and the observed community reality. A human validator should confirm the exact clause, responsible actor, deadline, and current evidence."
            : "The report does not clearly identify a verifiable commitment yet. A validator should request the agreement, policy, promise, or public commitment that anchors the accountability claim.";

        var reparativeProposal = report.IssueType switch
        {
            IssueType.BrokenPromise or IssueType.ServiceGap => $"Immediate verification within 14 days, public response from the responsible institution, and a community-validated repair plan. Demo estimate: {householdCount} households x $2/day x {daysOfHarm} days = {estimatedCost.ToString("C0", CultureInfo.GetCultureInfo("en-US"))}.",
            IssueType.EnvironmentalHarm => "Immediate safety assessment, temporary mitigation, community health screening, and a remediation plan validated by local representatives.",
            IssueType.SecurityConcern => "Immediate safety referral to trusted local actors, anonymized handling, and escalation only after consent and risk review.",
            _ => "Community validation session, responsible actor identification, and a time-bound response pathway before institutional escalation."
        };

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
            PolicyMatch = policyMatch,
            Steps =
            [
                new PipelineStepDto
                {
                    Name = "Agent 1 - Evidence Structuring",
                    Purpose = "Convert raw community testimony into a structured intelligence record.",
                    Output = $"Classified as {FormatIssueType(report.IssueType)} with {report.Urgency.ToString().ToLowerInvariant()} urgency and {report.Location.Confidence.ToString().ToLowerInvariant()} location confidence.",
                    Confidence = 0.72m
                },
                new PipelineStepDto
                {
                    Name = "Agent 2 - Policy RAG Comparison",
                    Purpose = "Compare the report against seeded agreements, commitments, and regional frameworks.",
                    Output = policyMatch is not null
                        ? $"Matched {policyMatch.DocumentTitle} ({policyMatch.Similarity:0.00} similarity). Gap: {policyMatch.Gap}"
                        : hasCommitment
                            ? "Potential commitment reference detected; human reviewer should attach the exact document."
                        : "No clear commitment reference detected; report needs follow-up evidence before escalation.",
                    Confidence = policyMatch?.Similarity ?? (hasCommitment ? 0.70m : 0.48m)
                },
                new PipelineStepDto
                {
                    Name = "Agent 3 - Safety and Ethics Review",
                    Purpose = "Check sensitivity, consent requirements, and whether escalation could expose the reporter or community.",
                    Output = report.IsSensitive
                        ? "Sensitive handling required. Validator must confirm consent and anonymization before escalation."
                        : "No sensitivity flag was selected. Validator still confirms consent, location confidence, evidence quality, and reporter safety.",
                    Confidence = report.IsSensitive ? 0.69m : 0.76m
                },
                new PipelineStepDto
                {
                    Name = "Agent 4 - Accountability Brief Generation",
                    Purpose = "Generate a validation-gated accountability brief for advocates and OSF partners.",
                    Output = $"Drafted brief with gap analysis and reparative proposal: {reparativeProposal}",
                    Confidence = 0.76m
                }
            ],
            Flags = BuildFlags(report, hasCommitment),
            Citations = policyMatch is not null
                ? [$"{policyMatch.DocumentTitle}: {policyMatch.Commitment}"]
                : hasCommitment ? ["Seed CDA demo clause: Section 4.2 - community water access commitment"] : []
        });
    }

    private static string BuildBrief(ReportDto report, string summary, string gapAnalysis, string reparativeProposal, PolicyMatchDto? policyMatch)
    {
        return $"""
        # Accountability Brief: {report.Location.Name}

        **Generated**: {DateTimeOffset.UtcNow:O}  
        **Community**: {report.Location.Name}  
        **Report ID**: {report.Id}  
        **Human validation status**: {report.Status}

        ## 1. Executive Summary
        {summary}

        ## 2. Community Evidence
        - **Source**: Community-submitted report{(report.IsSensitive ? " (sensitive; identifiers should be anonymized)" : "")}
        - **Date Reported**: {report.SubmittedAt:yyyy-MM-dd}
        - **Key Claim**: {report.Description}
        - **Trust Score**: {report.TrustScore.Overall:0.00}/1.0 ({report.TrustScore.InternalConsistency})

        ## 3. Policy/Agreement Reference
        - **Document**: {policyMatch?.DocumentTitle ?? "Pending document attachment"}
        - **Relevant Clause**: {policyMatch?.Commitment ?? "Demo seed commitment, pending human verification"}
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

    private static string FormatIssueType(IssueType issueType)
    {
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
}
