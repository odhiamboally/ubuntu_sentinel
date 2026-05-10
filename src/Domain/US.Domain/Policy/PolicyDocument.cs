using US.Domain.Common;

namespace US.Domain.Policy;

public sealed class PolicyDocument : Entity
{
    public string Title { get; init; } = default!;
    public string RegionCode { get; init; } = default!;
    public DocumentType Type { get; init; }
    public string FullText { get; init; } = default!;
    public string[] Commitments { get; init; } = [];
}
