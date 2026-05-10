namespace US.SharedKernel.Contracts.Intake;

public sealed record UssdSessionRequest
{
    public string SessionId { get; init; } = Guid.NewGuid().ToString("N");
    public string PhoneNumber { get; init; } = "+254700000000";
    public string ServiceCode { get; init; } = "*384#";
    public string Text { get; init; } = string.Empty;
}
