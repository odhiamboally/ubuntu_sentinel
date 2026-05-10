using US.SharedKernel.Contracts.Reports;

namespace US.Api.Features.Accountability;

public interface IAccountabilityPipeline
{
    Task<ReportPipelineResultDto> AnalyzeAsync(ReportDto report, CancellationToken cancellationToken);
}
