using System.Net.Http.Json;
using US.SharedKernel.Contracts.Regions;
using US.SharedKernel.Contracts.Reports;

namespace US.Web.Client.Services;

public sealed class UbuntuSentinelApiClient(HttpClient httpClient)
{
    public async Task<IReadOnlyList<RegionProfileDto>> GetRegionsAsync(CancellationToken cancellationToken = default)
    {
        return await httpClient.GetFromJsonAsync<IReadOnlyList<RegionProfileDto>>("/api/regions", cancellationToken)
            ?? [];
    }

    public async Task<IReadOnlyList<ReportDto>> GetReportsAsync(CancellationToken cancellationToken = default)
    {
        return await httpClient.GetFromJsonAsync<IReadOnlyList<ReportDto>>("/api/reports", cancellationToken)
            ?? [];
    }

    public async Task<ReportDto> SubmitReportAsync(SubmitReportRequest request, CancellationToken cancellationToken = default)
    {
        var response = await httpClient.PostAsJsonAsync("/api/reports", request, cancellationToken);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<ReportDto>(cancellationToken)
            ?? throw new InvalidOperationException("The API returned an empty report response.");
    }

    public async Task<ReportDto> ValidateReportAsync(Guid reportId, ValidateReportRequest request, CancellationToken cancellationToken = default)
    {
        var response = await httpClient.PostAsJsonAsync($"/api/reports/{reportId}/validation", request, cancellationToken);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<ReportDto>(cancellationToken)
            ?? throw new InvalidOperationException("The API returned an empty validation response.");
    }

    public async Task<ReportPipelineResultDto> GetPipelineResultAsync(Guid reportId, CancellationToken cancellationToken = default)
    {
        return await httpClient.GetFromJsonAsync<ReportPipelineResultDto>($"/api/reports/{reportId}/pipeline", cancellationToken)
            ?? throw new InvalidOperationException("The API returned an empty pipeline response.");
    }
}
