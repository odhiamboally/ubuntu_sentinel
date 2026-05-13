using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.Extensions.Options;

namespace US.Api.Features.Accountability;

public sealed class OpenAiChatJsonClient(HttpClient httpClient, IOptions<OpenAiPipelineOptions> options)
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly OpenAiPipelineOptions _options = options.Value;

    public bool IsConfigured => !string.IsNullOrWhiteSpace(_options.ApiKey);

    public string Model => _options.Model;

    public async Task<OpenAiDiagnosticsDto> GetDiagnosticsAsync(bool liveProbe, CancellationToken cancellationToken)
    {
        if (!IsConfigured)
        {
            return new OpenAiDiagnosticsDto
            {
                IsConfigured = false,
                Model = _options.Model,
                Endpoint = "chat_completions",
                RequestTimeoutSeconds = ClampTimeoutSeconds(),
                MaxRetries = ClampMaxRetries(),
                RetryBaseDelayMilliseconds = ClampRetryBaseDelayMilliseconds(),
                LiveProbeRequested = liveProbe,
                LiveProbeSucceeded = false,
                LiveProbeError = "OPENAI_API_KEY is not configured."
            };
        }

        if (!liveProbe)
        {
            return new OpenAiDiagnosticsDto
            {
                IsConfigured = true,
                Model = _options.Model,
                Endpoint = "chat_completions",
                RequestTimeoutSeconds = ClampTimeoutSeconds(),
                MaxRetries = ClampMaxRetries(),
                RetryBaseDelayMilliseconds = ClampRetryBaseDelayMilliseconds(),
                LiveProbeRequested = false,
                LiveProbeSucceeded = false
            };
        }

        try
        {
            var probe = await CompleteJsonAsync<ProbeResponse>(
                """
                You are a connectivity probe.
                Return JSON only with: ok, model_echo.
                Set ok=true and model_echo to the exact model input.
                """,
                new
                {
                    model = _options.Model,
                    utcNow = DateTimeOffset.UtcNow
                },
                cancellationToken);

            if (probe.Ok)
            {
                logger.LogInformation("OpenAI live probe succeeded for model {Model}.", _options.Model);
            }

            return new OpenAiDiagnosticsDto
            {
                IsConfigured = true,
                Model = _options.Model,
                Endpoint = "chat_completions",
                RequestTimeoutSeconds = ClampTimeoutSeconds(),
                MaxRetries = ClampMaxRetries(),
                RetryBaseDelayMilliseconds = ClampRetryBaseDelayMilliseconds(),
                LiveProbeRequested = true,
                LiveProbeSucceeded = probe.Ok,
                LiveProbeError = probe.Ok ? null : "Live OpenAI probe returned ok=false.",
                LiveProbeModelEcho = probe.ModelEcho
            };
        }
        catch (Exception exception)
        {
            logger.LogWarning(exception, "OpenAI diagnostics probe failed for model {Model}.", _options.Model);
            return new OpenAiDiagnosticsDto
            {
                IsConfigured = true,
                Model = _options.Model,
                Endpoint = "chat_completions",
                RequestTimeoutSeconds = ClampTimeoutSeconds(),
                MaxRetries = ClampMaxRetries(),
                RetryBaseDelayMilliseconds = ClampRetryBaseDelayMilliseconds(),
                LiveProbeRequested = true,
                LiveProbeSucceeded = false,
                LiveProbeError = exception.Message
            };
        }
    }

    public async Task<T> CompleteJsonAsync<T>(string systemPrompt, object input, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(_options.ApiKey))
        {
            throw new InvalidOperationException("OpenAI API key is not configured.");
        }

        using var request = new HttpRequestMessage(HttpMethod.Post, "https://api.openai.com/v1/chat/completions");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _options.ApiKey);
        request.Content = JsonContent.Create(new
        {
            model = _options.Model,
            temperature = 0.2,
            response_format = new { type = "json_object" },
            messages = new object[]
            {
                new { role = "system", content = systemPrompt },
                new { role = "user", content = JsonSerializer.Serialize(input, JsonOptions) }
            }
        }, options: JsonOptions);

        using var response = await httpClient.SendAsync(request, cancellationToken);
        var responseText = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            throw new InvalidOperationException($"OpenAI request failed with {(int)response.StatusCode}: {responseText}");
        }

        using var document = JsonDocument.Parse(responseText);
        var content = document
            .RootElement
            .GetProperty("choices")[0]
            .GetProperty("message")
            .GetProperty("content")
            .GetString();

        if (string.IsNullOrWhiteSpace(content))
        {
            throw new InvalidOperationException("OpenAI response did not include JSON content.");
        }

        return JsonSerializer.Deserialize<T>(content, JsonOptions)
            ?? throw new InvalidOperationException("OpenAI JSON content could not be deserialized.");
    }
}
