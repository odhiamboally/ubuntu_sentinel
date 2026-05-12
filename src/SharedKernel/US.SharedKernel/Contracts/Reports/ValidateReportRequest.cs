using US.SharedKernel.Enums;

namespace US.SharedKernel.Contracts.Reports;

public sealed record ValidateReportRequest
{
    public ValidationDecision Decision { get; init; }
    public string? Notes { get; init; }
    public ValidationChecksDto Checks { get; init; } = new();
}
