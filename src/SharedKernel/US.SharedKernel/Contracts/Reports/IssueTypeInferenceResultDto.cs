using US.SharedKernel.Enums;

namespace US.SharedKernel.Contracts.Reports;

public sealed record IssueTypeInferenceResultDto
{
    public IssueType? SuggestedType { get; init; }
    public decimal Confidence { get; init; }
    public IReadOnlyList<string> Signals { get; init; } = [];
    public InferenceSource Source { get; init; } = InferenceSource.Deterministic;

    public ConfidenceTier Tier => Confidence switch
    {
        >= 0.65m => ConfidenceTier.High,
        >= 0.35m => ConfidenceTier.Medium,
        _ => ConfidenceTier.Low
    };

    public static IssueTypeInferenceResultDto Uncertain() => new()
    {
        Confidence = 0m,
        Signals = []
    };
}
