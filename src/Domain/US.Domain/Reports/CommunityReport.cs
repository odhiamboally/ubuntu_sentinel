using US.Domain.Common;
using US.SharedKernel.Enums;

namespace US.Domain.Reports;

public sealed class CommunityReport : AggregateRoot
{
    private CommunityReport()
    {
    }

    public string RegionCode { get; private init; } = default!;
    public string LanguageCode { get; private init; } = "en";
    public string LocationName { get; private init; } = default!;
    public string Description { get; private init; } = default!;
    public IssueType IssueType { get; private init; }
    public UrgencyLevel Urgency { get; private set; } = UrgencyLevel.Medium;
    public ReportStatus Status { get; private set; } = ReportStatus.PendingValidation;
    public IntakeChannel IntakeChannel { get; private init; }
    public bool IsWomenLed { get; private init; }
    public bool IsYouthLed { get; private init; }
    public bool IsSensitive { get; private init; }
    public bool ConsentConfirmed { get; private set; }
    public DateTimeOffset SubmittedAt { get; private init; } = DateTimeOffset.UtcNow;
    public string? ValidatorNotes { get; private set; }

    public static CommunityReport Create(
        string regionCode,
        string languageCode,
        string locationName,
        string description,
        IssueType issueType,
        IntakeChannel intakeChannel,
        bool isWomenLed,
        bool isYouthLed,
        bool isSensitive,
        bool consentConfirmed)
    {
        return new CommunityReport
        {
            RegionCode = regionCode,
            LanguageCode = languageCode,
            LocationName = locationName,
            Description = description,
            IssueType = issueType,
            IntakeChannel = intakeChannel,
            IsWomenLed = isWomenLed,
            IsYouthLed = isYouthLed,
            IsSensitive = isSensitive,
            ConsentConfirmed = consentConfirmed,
            Urgency = description.Contains("attack", StringComparison.OrdinalIgnoreCase) ||
                      description.Contains("violence", StringComparison.OrdinalIgnoreCase)
                ? UrgencyLevel.High
                : UrgencyLevel.Medium
        };
    }

    public void ApplyValidation(ValidationDecision decision, string? notes)
    {
        Status = decision switch
        {
            ValidationDecision.Approve => ReportStatus.Approved,
            ValidationDecision.NeedsFollowUp => ReportStatus.NeedsFollowUp,
            ValidationDecision.Reject => ReportStatus.Rejected,
            _ => Status
        };

        ValidatorNotes = notes;
    }
}
