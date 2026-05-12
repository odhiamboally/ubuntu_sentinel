namespace US.SharedKernel.Contracts.Reports;

public sealed record PolicyDocumentDto
{
    public string Title { get; init; } = string.Empty;
    public string Type { get; init; } = string.Empty;
    public string RegionCode { get; init; } = string.Empty;
    public string Source { get; init; } = string.Empty;
    public string SourceUrl { get; init; } = string.Empty;
    public string ArticleReference { get; init; } = string.Empty;
    public string Commitment { get; init; } = string.Empty;
    public IReadOnlyList<string> IssueTypes { get; init; } = [];
    public IReadOnlyList<string> Keywords { get; init; } = [];
    public IReadOnlyList<string> RequiredTermsAny { get; init; } = [];
    public IReadOnlyList<string> PenaltyUnlessTermsAny { get; init; } = [];
    public decimal PenaltyWhenMissingTerms { get; init; } = 6m;
}
