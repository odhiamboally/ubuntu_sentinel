using US.SharedKernel.Enums;

namespace US.SharedKernel.Contracts.Reports;

public sealed record ReportPipelineResultDto
{
    public Guid ReportId { get; init; }
    public IssueType IssueType { get; init; }
    public UrgencyLevel Urgency { get; init; }
    public decimal Confidence { get; init; }
    public string Summary { get; init; } = string.Empty;
    public string GapAnalysis { get; init; } = string.Empty;
    public string ReparativeProposal { get; init; } = string.Empty;
    public string AccountabilityBriefMarkdown { get; init; } = string.Empty;
    public string PipelineMode { get; init; } = "Deterministic fallback";
    public string? PipelineModel { get; init; }
    public string? FallbackReason { get; init; }
    public PolicyMatchDto? PolicyMatch { get; init; }
    public IReadOnlyList<PipelineStepDto> Steps { get; init; } = [];
    public IReadOnlyList<string> Flags { get; init; } = [];
    public IReadOnlyList<string> Citations { get; init; } = [];
}
