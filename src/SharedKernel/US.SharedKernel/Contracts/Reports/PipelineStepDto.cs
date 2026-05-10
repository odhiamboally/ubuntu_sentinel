namespace US.SharedKernel.Contracts.Reports;

public sealed record PipelineStepDto
{
    public string Name { get; init; } = string.Empty;
    public string Purpose { get; init; } = string.Empty;
    public string Output { get; init; } = string.Empty;
    public decimal Confidence { get; init; }
}
