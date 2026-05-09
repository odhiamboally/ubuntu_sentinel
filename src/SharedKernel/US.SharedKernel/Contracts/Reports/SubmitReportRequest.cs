using System.ComponentModel.DataAnnotations;
using US.SharedKernel.Enums;

namespace US.SharedKernel.Contracts.Reports;

public sealed record SubmitReportRequest
{
    [Required]
    [StringLength(4_000, MinimumLength = 10)]
    public string Description { get; init; } = string.Empty;

    [StringLength(160)]
    public string LocationName { get; init; } = string.Empty;

    [StringLength(80)]
    public string RegionCode { get; init; } = "drc";

    [StringLength(12)]
    public string LanguageCode { get; init; } = "en";

    public IssueType? IssueTypeHint { get; init; }
    public bool WomenLed { get; init; }
    public bool YouthLed { get; init; }
    public bool IsSensitive { get; init; }
}
