namespace US.SharedKernel.Contracts.Regions;

public sealed record LanguageOptionDto
{
    public string Code { get; init; } = "en";
    public string Name { get; init; } = "English";
    public bool IsDefault { get; init; }
}
