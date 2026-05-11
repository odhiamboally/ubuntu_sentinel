using US.SharedKernel.Contracts.Reports;

namespace US.Api.Features.Accountability;

public interface IPolicyComparisonService
{
    PolicyMatchDto? FindBestMatch(ReportDto report);
}
