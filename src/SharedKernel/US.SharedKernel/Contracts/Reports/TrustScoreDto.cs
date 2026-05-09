using US.SharedKernel.Enums;

namespace US.SharedKernel.Contracts.Reports;

public sealed record TrustScoreDto
{
    public bool MetadataVerified { get; init; }
    public ConfidenceLevel InternalConsistency { get; init; } = ConfidenceLevel.Medium;
    public decimal Overall { get; init; }
}
