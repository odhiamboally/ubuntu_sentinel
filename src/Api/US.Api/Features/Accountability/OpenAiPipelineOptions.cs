namespace US.Api.Features.Accountability;

public sealed class OpenAiPipelineOptions
{
    public string? ApiKey { get; set; }
    public string Model { get; set; } = "gpt-4.1-mini";
    public int RequestTimeoutSeconds { get; set; } = 45;
    public int MaxRetries { get; set; } = 2;
    public int RetryBaseDelayMilliseconds { get; set; } = 500;
}
