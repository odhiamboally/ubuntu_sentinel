using US.Api.Features.Reports;
using US.SharedKernel.Contracts.Reports;

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

            var audience = ResolveAudience(httpContext);
            if (audience != BriefAudience.Admin)
            {
                return Results.Json(new
                {
                    generatedAt = DateTimeOffset.UtcNow,
                    exportType = "Ubuntu Sentinel accountability brief",
                    restricted = true,
                    message = "Accountability brief exports are available to administrators only."
                });
            }

            var result = await pipeline.AnalyzeAsync(report, cancellationToken);
            var fileName = $"{BuildBriefFileStem(report)}.json";

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

        endpoints.MapGet("/api/reports/{id:guid}/brief.pdf", async (
            Guid id,
            HttpContext httpContext,
            IReportStore reportStore,
            IAccountabilityPipeline pipeline,
            IBriefPdfRenderer pdfRenderer,
            CancellationToken cancellationToken) =>
        {
            var report = await reportStore.GetByIdAsync(id, cancellationToken);
            if (report is null)
            {
                return Results.NotFound();
            }

            var audience = ResolveAudience(httpContext);
            var result = audience == BriefAudience.Admin
                ? await pipeline.AnalyzeAsync(report, cancellationToken)
                : new ReportPipelineResultDto
                {
                    ReportId = report.Id,
                    IssueType = report.IssueType,
                    Urgency = report.Urgency
                };
            var pdf = pdfRenderer.Render(report, result, audience);

            return Results.File(
                pdf,
                "application/pdf",
                $"{BuildBriefFileStem(report)}.pdf");
        })
        .WithTags("Accountability");

        return endpoints;
    }

    private static BriefAudience ResolveAudience(HttpContext httpContext)
    {
        var audience = httpContext.Request.Query["audience"].ToString();
        return string.Equals(audience, "admin", StringComparison.OrdinalIgnoreCase)
            ? BriefAudience.Admin
            : BriefAudience.Restricted;
    }

    private static string BuildBriefFileStem(ReportDto report)
    {
        var location = new string(report.Location.Name
            .ToLowerInvariant()
            .Select(character => char.IsLetterOrDigit(character) ? character : '-')
            .ToArray());

        while (location.Contains("--", StringComparison.Ordinal))
        {
            location = location.Replace("--", "-", StringComparison.Ordinal);
        }

        location = location.Trim('-');
        if (string.IsNullOrWhiteSpace(location))
        {
            location = "community";
        }

        return $"ubuntu-sentinel-brief-{location}-{report.Id:N}";
    }
}
