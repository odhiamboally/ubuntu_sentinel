using US.SharedKernel.Contracts.Reports;

namespace US.Api.Features.Accountability;

public sealed class SeededPolicyComparisonService : IPolicyComparisonService
{
    private readonly IReadOnlyList<SeedPolicyDocument> _documents =
    [
        new(
            "Sahel Community Water Access Commitment",
            "Community Development Agreement",
            "sahel",
            "The mining operator shall fund and complete a clean community water point within six months of extraction activity beginning.",
            ["water", "well", "mining", "promise", "agreement", "families", "village"]),
        new(
            "Eastern DRC Community Benefit Protocol",
            "Mining License Addendum",
            "drc",
            "Extraction activities must be accompanied by community benefit delivery, local consultation, and public reporting on agreed services.",
            ["extraction", "mining", "benefit", "service", "consultation", "land", "drc"]),
        new(
            "Mozambique Recovery and Resilience Framework",
            "Regional Framework",
            "mozambique",
            "District recovery partners should document youth-led early warning networks and community resilience practices alongside incident reports.",
            ["youth", "warning", "recovery", "resilience", "safe route", "mozambique"]),
        new(
            "Sudan Displacement Safety Protocol",
            "Humanitarian Protection Protocol",
            "sudan",
            "Reports concerning displacement, safety, or humanitarian access must preserve anonymity and require consent before escalation.",
            ["displacement", "safety", "humanitarian", "anonymous", "consent", "sudan"])
    ];

    public PolicyMatchDto? FindBestMatch(ReportDto report)
    {
        var reportTerms = Tokenize($"{report.RegionCode} {report.Location.Name} {report.Description} {report.IssueType}");
        var ranked = _documents
            .Select(document => new
            {
                Document = document,
                Score = Score(document, reportTerms, report.RegionCode)
            })
            .OrderByDescending(item => item.Score)
            .FirstOrDefault();

        if (ranked is null || ranked.Score <= 0)
        {
            return null;
        }

        return new PolicyMatchDto
        {
            DocumentTitle = ranked.Document.Title,
            DocumentType = ranked.Document.Type,
            RegionCode = ranked.Document.RegionCode,
            Commitment = ranked.Document.Commitment,
            Gap = BuildGap(report, ranked.Document),
            Similarity = Math.Min(0.96m, 0.48m + ranked.Score / 20m)
        };
    }

    private static decimal Score(SeedPolicyDocument document, HashSet<string> reportTerms, string regionCode)
    {
        var keywordHits = document.Keywords.Count(keyword => reportTerms.Contains(keyword));
        var regionBoost = string.Equals(document.RegionCode, regionCode, StringComparison.OrdinalIgnoreCase) ? 4 : 0;
        return keywordHits + regionBoost;
    }

    private static string BuildGap(ReportDto report, SeedPolicyDocument document)
    {
        return $"Community testimony from {report.Location.Name} indicates the commitment may not be fulfilled: {document.Commitment}";
    }

    private static HashSet<string> Tokenize(string value)
    {
        return value
            .ToLowerInvariant()
            .Split([' ', ',', '.', ';', ':', '-', '_', '/', '\\', '(', ')'], StringSplitOptions.RemoveEmptyEntries)
            .Select(token => token.Trim())
            .Where(token => token.Length > 2)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
    }

    private sealed record SeedPolicyDocument(
        string Title,
        string Type,
        string RegionCode,
        string Commitment,
        IReadOnlyList<string> Keywords);
}
