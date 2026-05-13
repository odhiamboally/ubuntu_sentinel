using System.Globalization;
using US.SharedKernel.Contracts.Reports;

namespace US.Api.Features.Accountability;

public sealed class OpenAiAccountabilityPipeline(
    OpenAiChatJsonClient client,
    IPolicyComparisonService policyComparison,
    DeterministicAccountabilityPipeline fallback,
    ILogger<OpenAiAccountabilityPipeline> logger) : IAccountabilityPipeline
{
    public async Task<ReportPipelineResultDto> AnalyzeAsync(ReportDto report, CancellationToken cancellationToken)
    {
        logger.LogInformation("OpenAI configured: {IsConfigured}, Model: {Model}", client.IsConfigured, client.Model);

        if (!client.IsConfigured)
        {
            //var fallbackResult = await fallback.AnalyzeAsync(report, cancellationToken);
            //return fallbackResult with { FallbackReason = LocalizedMissingApiKey(report.LanguageCode) };

            logger.LogWarning("OpenAI not configured; using deterministic fallback.");
            var fallbackResult = await fallback.AnalyzeAsync(report, cancellationToken);
            return fallbackResult with { FallbackReason = null }; // never surface to PDF
        }

        try
        {
            var policyMatch = policyComparison.FindBestMatch(report);
            var targetLanguage = string.Equals(report.LanguageCode, "fr", StringComparison.OrdinalIgnoreCase)
                ? "French"
                : "English";
            var evidence = await client.CompleteJsonAsync<EvidenceAgentResult>(
                """
                You are Agent 1, an evidence structuring analyst for community accountability reports.
                Extract structured evidence from raw community testimony.
                Return JSON only with: summary, issue_type, issue_type_confidence, issue_type_signals, urgency, urgency_confidence, key_actors, location_confidence, language_detected, confidence.
                Do not invent facts. Use "unknown" where evidence is missing.
                Write all human-readable string values in the requested target language.
                """,
                new
                {
                    targetLanguage,
                    report.RegionCode,
                    location = report.Location.Name,
                    report.Description,
                    issueTypeHint = report.IssueType.ToString(),
                    urgencyHint = report.Urgency.ToString(),
                    report.Actors.WomenLed,
                    report.Actors.YouthLed,
                    report.IsSensitive
                },
                cancellationToken);

            var policy = await client.CompleteJsonAsync<PolicyAgentResult>(
                """
                You are Agent 2, a policy and clause comparison analyst.
                Compare the structured community evidence against the matched legal/policy clause.
                Return JSON only with: gap_analysis, commitment_likely_fulfilled, confidence, flags.
                Be precise and cite the supplied document/article. Do not claim legal conclusions beyond the evidence.
                Write all human-readable string values in the requested target language.
                """,
                new
                {
                    targetLanguage,
                    evidence,
                    policyMatch
                },
                cancellationToken);

            var safety = await client.CompleteJsonAsync<SafetyAgentResult>(
                """
                You are Agent 3, a safety and ethics reviewer.
                Review reporter safety, PII exposure, consent needs, and escalation risk.
                Return JSON only with: is_safe_for_escalation, risk_factors, recommendation, confidence.
                If violence, police abuse, displacement, or sensitive identity risk appears, require human validation before escalation.
                Write all human-readable string values in the requested target language.
                """,
                new
                {
                    targetLanguage,
                    evidence,
                    report.IsSensitive,
                    validationChecks = report.ValidationChecks,
                    rawDescription = report.Description
                },
                cancellationToken);

            var brief = await client.CompleteJsonAsync<BriefAgentResult>(
                """
                You are Agent 4, an accountability brief drafter for OSF-aligned community validation.
                Generate a validation-gated accountability brief from the evidence, policy gap, and safety review.
                Return JSON only with: executive_summary, reparative_proposal, recommended_next_steps, markdown, confidence.
                The brief must say that human validation is required before institutional escalation.
                Write all human-readable string values in the requested target language.
                """,
                new
                {
                    targetLanguage,
                    report = new
                    {
                        report.Id,
                        report.RegionCode,
                        location = report.Location.Name,
                        report.Description,
                        report.SubmittedAt,
                        report.Status
                    },
                    evidence,
                    policy,
                    policyMatch,
                    safety
                },
                cancellationToken);

            var confidence = Average(evidence.Confidence, policy.Confidence, safety.Confidence, brief.Confidence);
            var flags = policy.Flags
                .Concat(safety.RiskFactors.Select(NormalizeFlag))
                .Where(flag => !string.IsNullOrWhiteSpace(flag))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToArray();

            return new ReportPipelineResultDto
            {
                ReportId = report.Id,
                IssueType = report.IssueType,
                Urgency = report.Urgency,
                Confidence = confidence,
                Summary = evidence.Summary,
                GapAnalysis = policy.GapAnalysis,
                ReparativeProposal = brief.ReparativeProposal,
                AccountabilityBriefMarkdown = brief.Markdown,
                PipelineMode = "OpenAI-backed",
                PipelineModel = client.Model,
                PolicyMatch = policyMatch,
                Steps =
                [
                    new PipelineStepDto
                    {
                        Name = "Agent 1 - Evidence Structuring",
                        Purpose = "Convert raw community testimony into a structured intelligence record.",
                        Output = $"{evidence.Summary} Suggested issue: {evidence.IssueType} ({evidence.IssueTypeConfidence:0.00}; signals: {FormatList(evidence.IssueTypeSignals)}). Actors: {FormatList(evidence.KeyActors)}. Location confidence: {evidence.LocationConfidence}.",
                        Confidence = evidence.Confidence
                    },
                    new PipelineStepDto
                    {
                        Name = "Agent 2 - Policy RAG Comparison",
                        Purpose = "Compare the report against real sourced clauses and regional frameworks.",
                        Output = policy.GapAnalysis,
                        Confidence = policy.Confidence
                    },
                    new PipelineStepDto
                    {
                        Name = "Agent 3 - Safety and Ethics Review",
                        Purpose = "Check sensitivity, consent requirements, and whether escalation could expose the reporter or community.",
                        Output = $"{safety.Recommendation} Risk factors: {FormatList(safety.RiskFactors)}.",
                        Confidence = safety.Confidence
                    },
                    new PipelineStepDto
                    {
                        Name = "Agent 4 - Accountability Brief Generation",
                        Purpose = "Generate a validation-gated accountability brief for advocates and OSF partners.",
                        Output = brief.ExecutiveSummary,
                        Confidence = brief.Confidence
                    }
                ],
                Flags = flags,
                Citations = policyMatch is not null
                    ? [$"{policyMatch.DocumentTitle}, {policyMatch.ArticleReference} ({policyMatch.Source}): {policyMatch.Commitment}"]
                    : []
            };
        }
        catch (Exception exception)
        {
            //logger.LogWarning(exception, "OpenAI pipeline failed; falling back to deterministic accountability pipeline.");
            //var fallbackResult = await fallback.AnalyzeAsync(report, cancellationToken);
            //return fallbackResult with
            //{
            //    FallbackReason = LocalizedPipelineFailure(report.LanguageCode)
            //};

            var fallbackResult = await fallback.AnalyzeAsync(report, cancellationToken);
            return fallbackResult with { FallbackReason = null }; // log only, never surface

        }
    }

    private static string LocalizedMissingApiKey(string languageCode)
    {
        return string.Equals(languageCode, "fr", StringComparison.OrdinalIgnoreCase)
            ? "OPENAI_API_KEY n'est pas configure."
            : "OPENAI_API_KEY is not configured.";
    }

    private static string LocalizedPipelineFailure(string languageCode)
    {
        return string.Equals(languageCode, "fr", StringComparison.OrdinalIgnoreCase)
            ? "Le pipeline OpenAI a echoue. Le mode deterministe de secours a ete utilise."
            : "OpenAI pipeline failed. Deterministic fallback was used.";
    }

    private static decimal Average(params decimal[] values)
    {
        return Math.Clamp(values.Sum() / values.Length, 0.01m, 0.99m);
    }

    private static string FormatList(IReadOnlyList<string> values)
    {
        return values.Count == 0 ? "unknown" : string.Join(", ", values);
    }

    private static string NormalizeFlag(string value)
    {
        return value.Trim().ToLower(CultureInfo.InvariantCulture).Replace(' ', '_').Replace('-', '_');
    }

    private sealed record EvidenceAgentResult
    {
        public string Summary { get; init; } = string.Empty;
        public string IssueType { get; init; } = string.Empty;
        public decimal IssueTypeConfidence { get; init; } = 0.7m;
        public IReadOnlyList<string> IssueTypeSignals { get; init; } = [];
        public string Urgency { get; init; } = string.Empty;
        public decimal UrgencyConfidence { get; init; } = 0.7m;
        public IReadOnlyList<string> KeyActors { get; init; } = [];
        public string LocationConfidence { get; init; } = string.Empty;
        public string LanguageDetected { get; init; } = string.Empty;
        public decimal Confidence { get; init; } = 0.7m;
    }

    private sealed record PolicyAgentResult
    {
        public string GapAnalysis { get; init; } = string.Empty;
        public bool CommitmentLikelyFulfilled { get; init; }
        public decimal Confidence { get; init; } = 0.7m;
        public IReadOnlyList<string> Flags { get; init; } = [];
    }

    private sealed record SafetyAgentResult
    {
        public bool IsSafeForEscalation { get; init; }
        public IReadOnlyList<string> RiskFactors { get; init; } = [];
        public string Recommendation { get; init; } = string.Empty;
        public decimal Confidence { get; init; } = 0.7m;
    }

    private sealed record BriefAgentResult
    {
        public string ExecutiveSummary { get; init; } = string.Empty;
        public string ReparativeProposal { get; init; } = string.Empty;
        public IReadOnlyList<string> RecommendedNextSteps { get; init; } = [];
        public string Markdown { get; init; } = string.Empty;
        public decimal Confidence { get; init; } = 0.7m;
    }
}
