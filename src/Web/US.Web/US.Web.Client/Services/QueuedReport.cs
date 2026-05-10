using US.SharedKernel.Contracts.Reports;

namespace US.Web.Client.Services;

public sealed record QueuedReport
{
    public Guid LocalId { get; init; } = Guid.NewGuid();
    public SubmitReportRequest Request { get; init; } = new();
    public DateTimeOffset QueuedAt { get; init; } = DateTimeOffset.UtcNow;
    public int Attempts { get; init; }
}
