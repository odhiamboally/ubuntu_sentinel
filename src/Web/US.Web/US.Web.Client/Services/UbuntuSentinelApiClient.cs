using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using US.SharedKernel.Contracts.Auth;
using US.SharedKernel.Contracts.Intake;
using US.SharedKernel.Contracts.Regions;
using US.SharedKernel.Contracts.Reports;

namespace US.Web.Client.Services;

public sealed class UbuntuSentinelApiClient(HttpClient httpClient)
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        Converters = { new JsonStringEnumConverter() }
    };

    public Uri ApiBaseUri => httpClient.BaseAddress
        ?? throw new InvalidOperationException("The API client was created without a base address.");

    public async Task<IReadOnlyList<RegionProfileDto>> GetRegionsAsync(CancellationToken cancellationToken = default)
    {
        return await httpClient.GetFromJsonAsync<IReadOnlyList<RegionProfileDto>>("/api/regions", JsonOptions, cancellationToken)
            ?? [];
    }

    public async Task<IReadOnlyList<ReportDto>> GetReportsAsync(CancellationToken cancellationToken = default)
    {
        return await httpClient.GetFromJsonAsync<IReadOnlyList<ReportDto>>("/api/reports", JsonOptions, cancellationToken)
            ?? [];
    }

    public async Task<ReportDto> GetReportAsync(Guid reportId, CancellationToken cancellationToken = default)
    {
        return await httpClient.GetFromJsonAsync<ReportDto>($"/api/reports/{reportId}", JsonOptions, cancellationToken)
            ?? throw new InvalidOperationException("The API returned an empty report response.");
    }

    public async Task<ReportDto> SubmitReportAsync(SubmitReportRequest request, CancellationToken cancellationToken = default)
    {
        var response = await httpClient.PostAsJsonAsync("/api/reports", request, JsonOptions, cancellationToken);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<ReportDto>(JsonOptions, cancellationToken)
            ?? throw new InvalidOperationException("The API returned an empty report response.");
    }

    public async Task<UssdSessionResponse> SendUssdAsync(UssdSessionRequest request, CancellationToken cancellationToken = default)
    {
        var response = await httpClient.PostAsJsonAsync("/api/intake/ussd", request, JsonOptions, cancellationToken);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<UssdSessionResponse>(JsonOptions, cancellationToken)
            ?? throw new InvalidOperationException("The API returned an empty USSD response.");
    }

    public async Task<ReportDto> ValidateReportAsync(Guid reportId, ValidateReportRequest request, CancellationToken cancellationToken = default)
    {
        var response = await httpClient.PostAsJsonAsync($"/api/reports/{reportId}/validation", request, JsonOptions, cancellationToken);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<ReportDto>(JsonOptions, cancellationToken)
            ?? throw new InvalidOperationException("The API returned an empty validation response.");
    }

    public async Task<ReportPipelineResultDto> GetPipelineResultAsync(Guid reportId, CancellationToken cancellationToken = default)
    {
        return await httpClient.GetFromJsonAsync<ReportPipelineResultDto>($"/api/reports/{reportId}/pipeline", JsonOptions, cancellationToken)
            ?? throw new InvalidOperationException("The API returned an empty pipeline response.");
    }

    public string GetBriefJsonUrl(Guid reportId, bool adminAudience = false)
    {
        var query = adminAudience ? "?audience=admin" : string.Empty;
        return new Uri(ApiBaseUri, $"/api/reports/{reportId}/brief.json{query}").ToString();
    }

    public string GetBriefPdfUrl(Guid reportId, bool adminAudience = false)
    {
        var query = adminAudience ? "?audience=admin" : string.Empty;
        return new Uri(ApiBaseUri, $"/api/reports/{reportId}/brief.pdf{query}").ToString();
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        var response = await httpClient.PostAsJsonAsync("/api/auth/login", request, JsonOptions, cancellationToken);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<LoginResponse>(JsonOptions, cancellationToken)
            ?? throw new InvalidOperationException("The API returned an empty login response.");
    }

    public async Task DeleteReportAsync(Guid reportId, CancellationToken cancellationToken = default)
    {
        var response = await httpClient.DeleteAsync($"/api/reports/{reportId}", cancellationToken);
        response.EnsureSuccessStatusCode();
    }
}
