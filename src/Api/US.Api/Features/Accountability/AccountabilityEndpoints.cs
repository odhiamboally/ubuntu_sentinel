using US.Api.Features.Reports;

namespace US.Api.Features.Accountability;

public static class AccountabilityEndpoints
{
    public static IEndpointRouteBuilder MapAccountabilityEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/api/reports/{id:guid}/pipeline", async (
            Guid id,
            IReportStore reportStore,
            IAccountabilityPipeline pipeline,
            CancellationToken cancellationToken) =>
        {
            var report = await reportStore.GetByIdAsync(id, cancellationToken);
            if (report is null)
            {
                return Results.NotFound();
            }

            var result = await pipeline.AnalyzeAsync(report, cancellationToken);
            return Results.Ok(result);
        })
        .WithTags("Accountability");

        endpoints.MapGet("/api/reports/{id:guid}/brief.json", async (
            Guid id,
            HttpContext httpContext,
            IReportStore reportStore,
            IAccountabilityPipeline pipeline,
            CancellationToken cancellationToken) =>
        {
            var report = await reportStore.GetByIdAsync(id, cancellationToken);
            if (report is null)
            {
                return Results.NotFound();
            }

            var result = await pipeline.AnalyzeAsync(report, cancellationToken);
            var fileName = $"ubuntu-sentinel-brief-{report.Location.Name}-{report.Id:N}.json"
                .Replace(' ', '-')
                .ToLowerInvariant();

            httpContext.Response.Headers.ContentDisposition = $"attachment; filename=\"{fileName}\"";

            return Results.Json(new
            {
                generatedAt = DateTimeOffset.UtcNow,
                exportType = "Ubuntu Sentinel accountability brief",
                report = new
                {
                    report.Id,
                    report.RegionCode,
                    location = report.Location.Name,
                    report.IssueType,
                    report.Urgency,
                    report.Status,
                    report.LanguageCode,
                    report.Description,
                    report.Actors.WomenLed,
                    report.Actors.YouthLed,
                    sensitive = report.IsSensitive,
                    report.SubmittedAt,
                    report.ValidatorNotes,
                    validationChecks = report.ValidationChecks
                },
                pipeline = new
                {
                    result.PipelineMode,
                    result.PipelineModel,
                    result.FallbackReason,
                    result.Confidence,
                    result.Summary,
                    result.GapAnalysis,
                    result.ReparativeProposal,
                    result.Flags,
                    result.Citations,
                    source = result.PolicyMatch,
                    steps = result.Steps
                },
                brief = new
                {
                    markdown = result.AccountabilityBriefMarkdown,
                    validationGate = report.Status == US.SharedKernel.Enums.ReportStatus.Approved && report.ValidationChecks.IsComplete
                        ? "validated"
                        : "not_escalation_ready"
                }
            });
        })
        .WithTags("Accountability");

        return endpoints;
    }
}
