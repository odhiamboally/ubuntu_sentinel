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

        return endpoints;
    }
}
