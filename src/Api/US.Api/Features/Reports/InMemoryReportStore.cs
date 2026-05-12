using System.Collections.Concurrent;
using System.Text.Json;
using System.Text.Json.Serialization;
using US.SharedKernel.Contracts.Reports;
using US.SharedKernel.Enums;
using US.SharedKernel.Inference;

namespace US.Api.Features.Reports;

public sealed class InMemoryReportStore(IssueTypeInferenceService issueTypeInference) : IReportStore
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        WriteIndented = true,
        Converters = { new JsonStringEnumConverter() }
    };

    private readonly ConcurrentDictionary<Guid, ReportDto> _reports = new();
    private readonly SemaphoreSlim _fileLock = new(1, 1);
    private readonly string _storePath = Path.Combine(AppContext.BaseDirectory, "data", "reports.json");
    private bool _loaded;

    public Task<ReportDto> CreateAsync(SubmitReportRequest request, CancellationToken cancellationToken)
    {
        EnsureLoaded();

        var inference = issueTypeInference.Infer(request.Description);
        var userSelectedIssueType = request.IssueTypeHint is null or IssueType.Other ? null : request.IssueTypeHint;
        var issueType = ResolveIssueType(userSelectedIssueType, inference);

        var report = new ReportDto
        {
            Id = Guid.CreateVersion7(),
            Description = request.Description.Trim(),
            Location = new LocationDto
            {
                Name = string.IsNullOrWhiteSpace(request.LocationName) ? "Unspecified location" : request.LocationName.Trim(),
                Confidence = string.IsNullOrWhiteSpace(request.LocationName) ? ConfidenceLevel.Low : ConfidenceLevel.Medium
            },
            RegionCode = request.RegionCode.Trim().ToLowerInvariant(),
            LanguageCode = request.LanguageCode.Trim().ToLowerInvariant(),
            IssueType = issueType,
            Urgency = request.UrgencyHint ?? InferUrgency(request.Description),
            Status = ReportStatus.PendingValidation,
            Actors = new CommunityActorDto
            {
                WomenLed = request.WomenLed,
                YouthLed = request.YouthLed
            },
            TrustScore = new TrustScoreDto
            {
                MetadataVerified = false,
                InternalConsistency = ConfidenceLevel.Medium,
                Overall = 0.62m
            },
            UserSelectedIssueType = userSelectedIssueType,
            IssueTypeInference = inference,
            IsSensitive = request.IsSensitive,
            SubmittedAt = DateTimeOffset.UtcNow
        };

        _reports[report.Id] = report;
        Persist();
        return Task.FromResult(report);
    }

    private static IssueType ResolveIssueType(IssueType? userSelectedIssueType, IssueTypeInferenceResultDto inference)
    {
        if (inference is { SuggestedType: { } suggestedType, Tier: ConfidenceTier.High })
        {
            return suggestedType;
        }

        if (userSelectedIssueType is { } selected && inference.Tier == ConfidenceTier.Low)
        {
            return selected;
        }

        if (inference.SuggestedType is { } inferred)
        {
            return inferred;
        }

        return userSelectedIssueType ?? IssueType.Other;
    }

    public Task<IReadOnlyList<ReportDto>> GetAllAsync(CancellationToken cancellationToken)
    {
        EnsureLoaded();

        var reports = _reports.Values
            .OrderByDescending(report => report.SubmittedAt)
            .ToList();

        return Task.FromResult<IReadOnlyList<ReportDto>>(reports);
    }

    public Task<ReportDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        EnsureLoaded();

        _reports.TryGetValue(id, out var report);
        return Task.FromResult(report);
    }

    public Task<ReportDto?> UpdateValidationAsync(Guid id, ValidateReportRequest request, CancellationToken cancellationToken)
    {
        EnsureLoaded();

        if (!_reports.TryGetValue(id, out var report))
        {
            return Task.FromResult<ReportDto?>(null);
        }

        var status = request.Decision switch
        {
            ValidationDecision.Approve => ReportStatus.Approved,
            ValidationDecision.NeedsFollowUp => ReportStatus.NeedsFollowUp,
            ValidationDecision.Reject => ReportStatus.Rejected,
            _ => ReportStatus.PendingValidation
        };

        var updated = report with
        {
            Status = status,
            ValidationDecision = request.Decision,
            ValidatorNotes = string.IsNullOrWhiteSpace(request.Notes) ? null : request.Notes.Trim(),
            ValidatedAt = DateTimeOffset.UtcNow,
            ValidationChecks = request.Decision == ValidationDecision.Reject
                ? new ValidationChecksDto()
                : request.Checks
        };

        _reports[id] = updated;
        Persist();
        return Task.FromResult<ReportDto?>(updated);
    }

    public Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        EnsureLoaded();

        var deleted = _reports.TryRemove(id, out _);
        if (deleted)
        {
            Persist();
        }

        return Task.FromResult(deleted);
    }

    private void EnsureLoaded()
    {
        if (_loaded)
        {
            return;
        }

        _fileLock.Wait();
        try
        {
            if (_loaded)
            {
                return;
            }

            Directory.CreateDirectory(Path.GetDirectoryName(_storePath)!);

            if (File.Exists(_storePath))
            {
                var json = File.ReadAllText(_storePath);
                var reports = JsonSerializer.Deserialize<List<ReportDto>>(json, JsonOptions) ?? [];
                foreach (var report in reports)
                {
                    _reports[report.Id] = report;
                }
            }

            _loaded = true;
        }
        finally
        {
            _fileLock.Release();
        }
    }

    private void Persist()
    {
        _fileLock.Wait();
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(_storePath)!);
            var reports = _reports.Values
                .OrderByDescending(report => report.SubmittedAt)
                .ToList();
            File.WriteAllText(_storePath, JsonSerializer.Serialize(reports, JsonOptions));
        }
        finally
        {
            _fileLock.Release();
        }
    }

    private static UrgencyLevel InferUrgency(string description)
    {
        var normalized = description.ToLowerInvariant();

        if (normalized.Contains("attack") || normalized.Contains("violence") || normalized.Contains("armed") || normalized.Contains("immediate"))
        {
            return UrgencyLevel.Critical;
        }

        if (normalized.Contains("unsafe") || normalized.Contains("threat") || normalized.Contains("blocked") || normalized.Contains("water"))
        {
            return UrgencyLevel.High;
        }

        return UrgencyLevel.Medium;
    }
}
