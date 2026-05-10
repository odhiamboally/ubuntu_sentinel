namespace US.SharedKernel.Contracts.Intake;

public sealed record UssdSessionResponse
{
    public string SessionId { get; init; } = string.Empty;
    public string Message { get; init; } = string.Empty;
    public bool IsTerminal { get; init; }
    public Guid? ReportId { get; init; }
    public string? ReportStatus { get; init; }
}
