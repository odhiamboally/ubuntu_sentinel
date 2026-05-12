namespace US.SharedKernel.Contracts.Reports;

public sealed record ValidationChecksDto
{
    public bool ConsentConfirmed { get; init; }
    public bool LocationConfidenceChecked { get; init; }
    public bool EvidenceQualityChecked { get; init; }
    public bool ReporterSafetyChecked { get; init; }

    public bool IsComplete =>
        ConsentConfirmed &&
        LocationConfidenceChecked &&
        EvidenceQualityChecked &&
        ReporterSafetyChecked;
}
