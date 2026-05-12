namespace US.SharedKernel.Contracts.Intake;

public sealed record UssdSubmissionDraft
{
    public string Flow { get; init; } = "incident";
    public string RegionCode { get; init; } = "sahel";
    public string LanguageCode { get; init; } = "fr";
    public string LocationName { get; init; } = string.Empty;
    public string IssueTypeCode { get; init; } = "broken-promise";
    public string UrgencyCode { get; init; } = "2";
    public string FlagsCode { get; init; } = "0";
    public string Description { get; init; } = string.Empty;
}
