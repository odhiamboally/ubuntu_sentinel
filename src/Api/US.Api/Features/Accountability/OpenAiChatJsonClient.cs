using System.Net.Http.Headers;
using System.Net;
using System.Text.Json;
using Microsoft.Extensions.Options;

namespace US.Api.Features.Accountability;

public sealed class OpenAiChatJsonClient(
    HttpClient httpClient,
    IOptions<OpenAiPipelineOptions> options,
    ILogger<OpenAiChatJsonClient> logger)
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

        var maxRetries = ClampMaxRetries();
        var timeout = TimeSpan.FromSeconds(ClampTimeoutSeconds());

        for (var attempt = 0; attempt <= maxRetries; attempt++)
        {
            using var timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            timeoutCts.CancelAfter(timeout);

            try
            {
                using var request = BuildRequest(systemPrompt, input);
                using var response = await httpClient.SendAsync(request, timeoutCts.Token);
                var responseText = await response.Content.ReadAsStringAsync(timeoutCts.Token);

                if (response.IsSuccessStatusCode)
                {
                    return DeserializeResponse<T>(responseText);
                }

                if (attempt < maxRetries && IsTransient(response.StatusCode))
                {
                    logger.LogWarning(
                        "OpenAI transient HTTP {StatusCode} on attempt {Attempt}/{MaxAttempts} for model {Model}. Retrying.",
                        (int)response.StatusCode,
                        attempt + 1,
                        maxRetries + 1,
                        _options.Model);
                    await DelayBeforeRetryAsync(attempt, cancellationToken);
                    continue;
                }

                throw new InvalidOperationException($"OpenAI request failed with {(int)response.StatusCode}: {Truncate(responseText, 600)}");
            }
            catch (OperationCanceledException exception) when (!cancellationToken.IsCancellationRequested)
            {
                if (attempt < maxRetries)
                {
                    logger.LogWarning(
                        exception,
                        "OpenAI request timed out after {TimeoutSeconds}s on attempt {Attempt}/{MaxAttempts} for model {Model}. Retrying.",
                        timeout.TotalSeconds,
                        attempt + 1,
                        maxRetries + 1,
                        _options.Model);
                    await DelayBeforeRetryAsync(attempt, cancellationToken);
                    continue;
                }

                throw new TimeoutException($"OpenAI request timed out after {timeout.TotalSeconds:0} seconds.", exception);
            }
            catch (HttpRequestException exception)
            {
                if (attempt < maxRetries)
                {
                    logger.LogWarning(
                        exception,
                        "OpenAI network error on attempt {Attempt}/{MaxAttempts} for model {Model}. Retrying.",
                        attempt + 1,
                        maxRetries + 1,
                        _options.Model);
                    await DelayBeforeRetryAsync(attempt, cancellationToken);
                    continue;
                }

                throw new InvalidOperationException("OpenAI request failed due to network error.", exception);
            }
        }

        throw new InvalidOperationException("OpenAI request did not complete successfully.");
    }

    private HttpRequestMessage BuildRequest(string systemPrompt, object input)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, "https://api.openai.com/v1/chat/completions");
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

        return request;
    }

    private static T DeserializeResponse<T>(string responseText)
    {
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

    private async Task DelayBeforeRetryAsync(int attempt, CancellationToken cancellationToken)
    {
        var baseDelay = ClampRetryBaseDelayMilliseconds();
        var delayMs = (int)Math.Min(baseDelay * Math.Pow(2, attempt), 5000);
        await Task.Delay(delayMs, cancellationToken);
    }

    private int ClampTimeoutSeconds() => Math.Clamp(_options.RequestTimeoutSeconds, 10, 180);

    private int ClampMaxRetries() => Math.Clamp(_options.MaxRetries, 0, 5);

    private int ClampRetryBaseDelayMilliseconds() => Math.Clamp(_options.RetryBaseDelayMilliseconds, 100, 5000);

    private static bool IsTransient(HttpStatusCode statusCode)
    {
        return statusCode is HttpStatusCode.RequestTimeout or HttpStatusCode.TooManyRequests
            || (int)statusCode >= 500;
    }

    private static string Truncate(string value, int maxLength)
    {
        return value.Length <= maxLength ? value : value[..maxLength];
    }

    private sealed record ProbeResponse
    {
        public bool Ok { get; init; }
        public string ModelEcho { get; init; } = string.Empty;
    }
}

public sealed class OpenAiDiagnosticsDto
{
    public bool IsConfigured { get; init; }
    public string Model { get; init; } = string.Empty;
    public string Endpoint { get; init; } = "chat_completions";
    public int RequestTimeoutSeconds { get; init; }
    public int MaxRetries { get; init; }
    public int RetryBaseDelayMilliseconds { get; init; }
    public bool LiveProbeRequested { get; init; }
    public bool LiveProbeSucceeded { get; init; }
    public string? LiveProbeError { get; init; }
    public string? LiveProbeModelEcho { get; init; }
}
