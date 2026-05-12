using System.Text.Json;
using System.Text.Json.Serialization;
using US.SharedKernel.Contracts.Regions;

namespace US.Api.Features.Regions;

public sealed class RegionProfileStore : IRegionProfileStore
{
    private readonly IReadOnlyList<RegionProfileDto> _regions;

    public RegionProfileStore(IWebHostEnvironment environment)
    {
        var path = Path.Combine(environment.ContentRootPath, "data", "region-profiles.json");
        var json = File.ReadAllText(path);
        var options = new JsonSerializerOptions(JsonSerializerDefaults.Web);
        options.Converters.Add(new JsonStringEnumConverter());
        _regions = JsonSerializer.Deserialize<IReadOnlyList<RegionProfileDto>>(json, options) ?? [];
    }

    public Task<IReadOnlyList<RegionProfileDto>> GetAllAsync(CancellationToken cancellationToken)
    {
        return Task.FromResult(_regions);
    }
}
