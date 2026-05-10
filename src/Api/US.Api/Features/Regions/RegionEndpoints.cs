namespace US.Api.Features.Regions;

public static class RegionEndpoints
{
    public static IEndpointRouteBuilder MapRegionEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/api/regions", async (IRegionProfileStore store, CancellationToken cancellationToken) =>
        {
            var regions = await store.GetAllAsync(cancellationToken);
            return Results.Ok(regions);
        })
        .WithTags("Regions");

        return endpoints;
    }
}
