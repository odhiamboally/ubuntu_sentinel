using System.Globalization;
using US.SharedKernel.Contracts.Reports;
using US.SharedKernel.Enums;

namespace US.Api.Features.Accountability;

public sealed class DeterministicAccountabilityPipeline : IAccountabilityPipeline
{
    public Task<ReportPipelineResultDto> AnalyzeAsync(ReportDto report, CancellationToken cancellationToken)
    {
        var householdCount = InferHouseholds(report.Description);
        var daysOfHarm = InferDays(report.Description);
        var estimatedCost = householdCount * 2 * daysOfHarm;
        var hasCommitment = !string.IsNullOrWhiteSpace(report.ReferencedCommitment)
            || report.Description.Contains("promised", StringComparison.OrdinalIgnoreCase)
            || report.Description.Contains("pledged", StringComparison.OrdinalIgnoreCase)
            || report.Description.Contains("agreement", StringComparison.OrdinalIgnoreCase)
            || report.Description.Contains("cda", StringComparison.OrdinalIgnoreCase);

        var summary = $"{report.Location.Name} submitted a {FormatIssueType(report.IssueType)} report with {report.Urgency.ToString().ToLowerInvariant()} urgency. The report is pending human validation before escalation.";
        var gapAnalysis = hasCommitment
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
            AccountabilityBriefMarkdown = BuildBrief(report, summary, gapAnalysis, reparativeProposal),
            Steps =
            [
                new PipelineStepDto
                {
                    Name = "Validation Agent",
                    Purpose = "Structure the raw community report and identify basic confidence signals.",
                    Output = $"Classified as {FormatIssueType(report.IssueType)} with {report.Urgency.ToString().ToLowerInvariant()} urgency and {report.Location.Confidence.ToString().ToLowerInvariant()} location confidence.",
                    Confidence = 0.72m
                },
                new PipelineStepDto
                {
                    Name = "Document Intelligence Agent",
                    Purpose = "Compare the report against seeded commitments or policy language.",
                    Output = hasCommitment
                        ? "Potential commitment reference detected; demo seed clause should be verified by a human reviewer."
                        : "No clear commitment reference detected; report needs follow-up evidence before escalation.",
                    Confidence = hasCommitment ? 0.70m : 0.48m
                },
                new PipelineStepDto
                {
                    Name = "Reparative Justice Calculator",
                    Purpose = "Translate harm signals into concrete, community-validated repair options.",
                    Output = reparativeProposal,
                    Confidence = 0.64m
                },
                new PipelineStepDto
                {
                    Name = "Advocacy Drafter",
                    Purpose = "Generate a concise accountability brief for validator review.",
                    Output = "Drafted export-ready markdown with evidence, gap analysis, proposal, next steps, and Ubuntu principle.",
                    Confidence = 0.76m
                }
            ],
            Flags = BuildFlags(report, hasCommitment),
            Citations = hasCommitment ? ["Seed CDA demo clause: Section 4.2 - community water access commitment"] : []
        });
    }

    private static string BuildBrief(ReportDto report, string summary, string gapAnalysis, string reparativeProposal)
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
        - **Relevant Clause**: Demo seed commitment, pending human verification
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
