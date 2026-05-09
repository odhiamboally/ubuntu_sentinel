using US.SharedKernel.Enums;

namespace US.SharedKernel.Contracts.Reports;

public sealed record LocationDto
{
    public string Name { get; init; } = string.Empty;
    public double? Latitude { get; init; }
    public double? Longitude { get; init; }
    public ConfidenceLevel Confidence { get; init; } = ConfidenceLevel.Low;
}
