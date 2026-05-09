using US.SharedKernel.Enums;

namespace US.SharedKernel.Contracts.Regions;

public sealed record RegionProfileDto
{
    public string Code { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public IReadOnlyList<LanguageOptionDto> Languages { get; init; } = [];
    public IReadOnlyList<IssueType> PriorityIssueTypes { get; init; } = [];
    public IReadOnlyList<string> RegionalBodies { get; init; } = [];
    public IReadOnlyList<string> EscalationPathways { get; init; } = [];
}
