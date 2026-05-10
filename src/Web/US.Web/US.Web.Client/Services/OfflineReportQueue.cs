using System.Text.Json;
using Microsoft.JSInterop;
using US.SharedKernel.Contracts.Reports;

namespace US.Web.Client.Services;

public sealed class OfflineReportQueue(IJSRuntime jsRuntime, UbuntuSentinelApiClient apiClient)
{
    private const string QueueKey = "ubuntu_sentinel.pending_reports";
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public async Task EnqueueAsync(SubmitReportRequest request, CancellationToken cancellationToken = default)
    {
        var queue = await GetQueueAsync(cancellationToken);
        queue.Add(new QueuedReport { Request = Copy(request) });
        await SaveQueueAsync(queue, cancellationToken);
    }

    public async Task<int> GetPendingCountAsync(CancellationToken cancellationToken = default)
    {
        return (await GetQueueAsync(cancellationToken)).Count;
    }

    public async Task<OfflineSyncResult> SyncAsync(CancellationToken cancellationToken = default)
    {
        var queue = await GetQueueAsync(cancellationToken);
        if (queue.Count == 0)
        {
            return new OfflineSyncResult(0, 0, 0);
        }

        var remaining = new List<QueuedReport>();
        var synced = 0;

        foreach (var item in queue)
        {
            try
            {
                await apiClient.SubmitReportAsync(item.Request, cancellationToken);
                synced++;
            }
            catch
            {
                remaining.Add(item with { Attempts = item.Attempts + 1 });
            }
        }

        await SaveQueueAsync(remaining, cancellationToken);
        return new OfflineSyncResult(queue.Count, synced, remaining.Count);
    }

    private async Task<List<QueuedReport>> GetQueueAsync(CancellationToken cancellationToken)
    {
        var json = await jsRuntime.InvokeAsync<string?>("localStorage.getItem", cancellationToken, QueueKey);

        if (string.IsNullOrWhiteSpace(json))
        {
            return [];
        }

        try
        {
            return JsonSerializer.Deserialize<List<QueuedReport>>(json, JsonOptions) ?? [];
        }
        catch
        {
            await jsRuntime.InvokeVoidAsync("localStorage.removeItem", cancellationToken, QueueKey);
            return [];
        }
    }

    private async Task SaveQueueAsync(IReadOnlyCollection<QueuedReport> queue, CancellationToken cancellationToken)
    {
        if (queue.Count == 0)
        {
            await jsRuntime.InvokeVoidAsync("localStorage.removeItem", cancellationToken, QueueKey);
            return;
        }

        var json = JsonSerializer.Serialize(queue, JsonOptions);
        await jsRuntime.InvokeVoidAsync("localStorage.setItem", cancellationToken, QueueKey, json);
    }

    private static SubmitReportRequest Copy(SubmitReportRequest request)
    {
        return new SubmitReportRequest
        {
            Description = request.Description,
            LocationName = request.LocationName,
            RegionCode = request.RegionCode,
            LanguageCode = request.LanguageCode,
            IssueTypeHint = request.IssueTypeHint,
            WomenLed = request.WomenLed,
            YouthLed = request.YouthLed,
            IsSensitive = request.IsSensitive
        };
    }
}

public sealed record OfflineSyncResult(int Attempted, int Synced, int Remaining);
