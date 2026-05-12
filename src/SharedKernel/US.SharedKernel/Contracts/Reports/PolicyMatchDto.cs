namespace US.SharedKernel.Contracts.Reports;

public sealed record PolicyMatchDto
{
    public string DocumentTitle { get; init; } = string.Empty;
    public string DocumentType { get; init; } = string.Empty;
    public string RegionCode { get; init; } = string.Empty;
    public string Source { get; init; } = string.Empty;
    public string SourceUrl { get; init; } = string.Empty;
    public string ArticleReference { get; init; } = string.Empty;
    public string Commitment { get; init; } = string.Empty;
    public string Gap { get; init; } = string.Empty;
    public decimal Similarity { get; init; }
}
