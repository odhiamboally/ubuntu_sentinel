using US.Domain.Common;
using US.SharedKernel.Enums;

namespace US.Domain.Peace;

public sealed class IntelligenceZone : Entity
{
    public string RegionCode { get; init; } = default!;
    public string Name { get; init; } = default!;
    public ZoneType Type { get; init; }
    public IssueType IssueType { get; init; }
    public string Summary { get; init; } = default!;
    public decimal Latitude { get; init; }
    public decimal Longitude { get; init; }
    public int EvidenceCount { get; init; }
    public bool IsWomenLed { get; init; }
    public bool IsYouthLed { get; init; }
}
