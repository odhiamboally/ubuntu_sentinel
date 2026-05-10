using US.SharedKernel.Contracts.Regions;

namespace US.Api.Features.Regions;

public interface IRegionProfileStore
{
    Task<IReadOnlyList<RegionProfileDto>> GetAllAsync(CancellationToken cancellationToken);
}
