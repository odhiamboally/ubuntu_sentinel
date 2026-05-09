using US.SharedKernel.Enums;

namespace US.SharedKernel.Contracts.Reports;

public sealed record ReportDto
{
    public Guid Id { get; init; }
    public string Description { get; init; } = string.Empty;
    public LocationDto Location { get; init; } = new();
    public string RegionCode { get; init; } = "drc";
    public string LanguageCode { get; init; } = "en";
    public IssueType IssueType { get; init; } = IssueType.Other;
    public UrgencyLevel Urgency { get; init; } = UrgencyLevel.Medium;
    public ReportStatus Status { get; init; } = ReportStatus.Submitted;
    public CommunityActorDto Actors { get; init; } = new();
    public TrustScoreDto TrustScore { get; init; } = new();
    public string? ReferencedCommitment { get; init; }
    public string? GapDescription { get; init; }
    public string? CommunityImpact { get; init; }
    public bool IsSensitive { get; init; }
    public DateTimeOffset SubmittedAt { get; init; }
    public DateTimeOffset? ValidatedAt { get; init; }
    public ValidationDecision ValidationDecision { get; init; } = ValidationDecision.Pending;
    public string? ValidatorNotes { get; init; }
}
