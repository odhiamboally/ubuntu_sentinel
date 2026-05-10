using System.Collections.Concurrent;
using US.SharedKernel.Contracts.Reports;
using US.SharedKernel.Enums;

namespace US.Api.Features.Reports;

public sealed class InMemoryReportStore : IReportStore
{
    private readonly ConcurrentDictionary<Guid, ReportDto> _reports = new();

    public Task<ReportDto> CreateAsync(SubmitReportRequest request, CancellationToken cancellationToken)
    {
        var report = new ReportDto
        {
            Id = Guid.NewGuid(),
            Description = request.Description.Trim(),
            Location = new LocationDto
            {
                Name = string.IsNullOrWhiteSpace(request.LocationName) ? "Unspecified location" : request.LocationName.Trim(),
                Confidence = string.IsNullOrWhiteSpace(request.LocationName) ? ConfidenceLevel.Low : ConfidenceLevel.Medium
            },
            RegionCode = request.RegionCode.Trim().ToLowerInvariant(),
            LanguageCode = request.LanguageCode.Trim().ToLowerInvariant(),
            IssueType = request.IssueTypeHint ?? IssueType.Other,
            Urgency = InferUrgency(request.Description),
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
            IsSensitive = request.IsSensitive,
            SubmittedAt = DateTimeOffset.UtcNow
        };

        _reports[report.Id] = report;
        return Task.FromResult(report);
    }

    public Task<IReadOnlyList<ReportDto>> GetAllAsync(CancellationToken cancellationToken)
    {
        var reports = _reports.Values
            .OrderByDescending(report => report.SubmittedAt)
            .ToList();

        return Task.FromResult<IReadOnlyList<ReportDto>>(reports);
    }

    public Task<ReportDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        _reports.TryGetValue(id, out var report);
        return Task.FromResult(report);
    }

    public Task<ReportDto?> UpdateValidationAsync(Guid id, ValidationDecision decision, string? notes, CancellationToken cancellationToken)
    {
        if (!_reports.TryGetValue(id, out var report))
        {
            return Task.FromResult<ReportDto?>(null);
        }

        var status = decision switch
        {
            ValidationDecision.Approve => ReportStatus.Approved,
            ValidationDecision.NeedsFollowUp => ReportStatus.NeedsFollowUp,
            ValidationDecision.Reject => ReportStatus.Rejected,
            _ => ReportStatus.PendingValidation
        };

        var updated = report with
        {
            Status = status,
            ValidationDecision = decision,
            ValidatorNotes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim(),
            ValidatedAt = DateTimeOffset.UtcNow
        };

        _reports[id] = updated;
        return Task.FromResult<ReportDto?>(updated);
    }

    public Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        return Task.FromResult(_reports.TryRemove(id, out _));
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
