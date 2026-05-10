using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using US.Api.Features.Realtime;
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
            IHubContext<ReportHub> hubContext,
            CancellationToken cancellationToken) =>
        {
            var report = await store.CreateAsync(request, cancellationToken);
            await hubContext.Clients.All.SendAsync("ReportSubmitted", report, cancellationToken);
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
            IHubContext<ReportHub> hubContext,
            CancellationToken cancellationToken) =>
        {
            var report = await store.UpdateValidationAsync(id, request.Decision, request.Notes, cancellationToken);
            if (report is null)
            {
                return Results.NotFound();
            }

            await hubContext.Clients.All.SendAsync("ReportValidated", report, cancellationToken);
            return Results.Ok(report);
        });

        group.MapDelete("/{id:guid}", async (
            Guid id,
            IReportStore store,
            IHubContext<ReportHub> hubContext,
            CancellationToken cancellationToken) =>
        {
            var deleted = await store.DeleteAsync(id, cancellationToken);
            if (!deleted)
            {
                return Results.NotFound();
            }

            await hubContext.Clients.All.SendAsync("ReportDeleted", id, cancellationToken);
            return Results.NoContent();
        });

        return endpoints;
    }
}
