namespace US.SharedKernel.Contracts.Regions;

public sealed record RegionSourceDto
{
    public string Title { get; init; } = string.Empty;
    public string Type { get; init; } = string.Empty;
    public string SourceUrl { get; init; } = string.Empty;
}
