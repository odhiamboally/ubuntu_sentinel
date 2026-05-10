using US.SharedKernel.Contracts.Reports;
using US.SharedKernel.Enums;

namespace US.Api.Features.Reports;

public interface IReportStore
{
    Task<ReportDto> CreateAsync(SubmitReportRequest request, CancellationToken cancellationToken);
    Task<IReadOnlyList<ReportDto>> GetAllAsync(CancellationToken cancellationToken);
    Task<ReportDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<ReportDto?> UpdateValidationAsync(Guid id, ValidationDecision decision, string? notes, CancellationToken cancellationToken);
}
