using System.ComponentModel.DataAnnotations;
using US.SharedKernel.Enums;

namespace US.SharedKernel.Contracts.Reports;

public sealed record SubmitReportRequest
{
    [Required]
    [StringLength(4_000, MinimumLength = 10)]
    public string Description { get; set; } = string.Empty;

    [StringLength(160)]
    public string LocationName { get; set; } = string.Empty;

    [StringLength(80)]
    public string RegionCode { get; set; } = "drc";

    [StringLength(12)]
    public string LanguageCode { get; set; } = "en";

    public IssueType? IssueTypeHint { get; set; }
    public UrgencyLevel? UrgencyHint { get; set; }
    public bool WomenLed { get; set; }
    public bool YouthLed { get; set; }
    public bool IsSensitive { get; set; }
}
