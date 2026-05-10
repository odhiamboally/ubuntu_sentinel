using Microsoft.AspNetCore.Mvc;
using US.SharedKernel.Contracts.Reports;

namespace US.Api.Features.Reports;

public static class ReportEndpoints
{
    public static IEndpointRouteBuilder MapReportEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/reports")
            .WithTags("Reports");

        group.MapPost("/", async (
            [FromBody] SubmitReportRequest request,
            IReportStore store,
            CancellationToken cancellationToken) =>
        {
            var report = await store.CreateAsync(request, cancellationToken);
            return Results.Created($"/api/reports/{report.Id}", report);
        });

        group.MapGet("/", async (IReportStore store, CancellationToken cancellationToken) =>
        {
            var reports = await store.GetAllAsync(cancellationToken);
            return Results.Ok(reports);
        });

        group.MapGet("/{id:guid}", async (Guid id, IReportStore store, CancellationToken cancellationToken) =>
        {
            var report = await store.GetByIdAsync(id, cancellationToken);
            return report is null ? Results.NotFound() : Results.Ok(report);
        });

        group.MapPost("/{id:guid}/validation", async (
            Guid id,
            [FromBody] ValidateReportRequest request,
            IReportStore store,
            CancellationToken cancellationToken) =>
        {
            var report = await store.UpdateValidationAsync(id, request.Decision, request.Notes, cancellationToken);
            return report is null ? Results.NotFound() : Results.Ok(report);
        });

        return endpoints;
    }
}
