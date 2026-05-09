namespace US.SharedKernel.Contracts.Reports;

public sealed record CommunityActorDto
{
    public bool WomenLed { get; init; }
    public bool YouthLed { get; init; }
    public string? CommunityGroup { get; init; }
}
