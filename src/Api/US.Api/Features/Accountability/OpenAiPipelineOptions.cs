namespace US.Api.Features.Accountability;

public sealed class OpenAiPipelineOptions
{
    public string? ApiKey { get; set; }
    public string Model { get; set; } = "gpt-4.1-mini";
}
