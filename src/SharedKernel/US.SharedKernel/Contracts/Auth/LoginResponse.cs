namespace US.SharedKernel.Contracts.Auth;

public sealed record LoginResponse
{
    public string Username { get; init; } = string.Empty;
    public string DisplayName { get; init; } = string.Empty;
    public string Role { get; init; } = string.Empty;
}
