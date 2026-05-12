using System.Text.Json;
using US.SharedKernel.Contracts.Reports;
using US.SharedKernel.Enums;

namespace US.Api.Features.Accountability;

public sealed class SeededPolicyComparisonService : IPolicyComparisonService
{
    private readonly IReadOnlyList<PolicyDocumentDto> _documents;

    public SeededPolicyComparisonService(IWebHostEnvironment environment)
    {
        var path = Path.Combine(environment.ContentRootPath, "data", "policy-documents.json");
        var json = File.ReadAllText(path);
        _documents = JsonSerializer.Deserialize<IReadOnlyList<PolicyDocumentDto>>(json, new JsonSerializerOptions(JsonSerializerDefaults.Web)) ?? [];
    }

    public IReadOnlyList<PolicyDocumentDto> GetDocuments() => _documents;

    public PolicyMatchDto? FindBestMatch(ReportDto report)
    {
        var reportText = $"{report.RegionCode} {report.Location.Name} {report.Description} {report.IssueType}";
        var reportTerms = Tokenize(reportText);
        var normalizedReport = Normalize(reportText);
        var ranked = _documents
            .Select(document => new
            {
                Document = document,
                Score = Score(document, reportTerms, normalizedReport, report.RegionCode, report.IssueType)
            })
            .OrderByDescending(item => item.Score)
            .FirstOrDefault();

        if (ranked is null || ranked.Score < 7)
        {
            return null;
        }

        return new PolicyMatchDto
        {
            DocumentTitle = ranked.Document.Title,
            DocumentType = ranked.Document.Type,
            RegionCode = ranked.Document.RegionCode,
            Source = ranked.Document.Source,
            SourceUrl = ranked.Document.SourceUrl,
            ArticleReference = ranked.Document.ArticleReference,
            Commitment = ranked.Document.Commitment,
            Gap = BuildGap(report, ranked.Document),
            Similarity = Math.Min(0.96m, 0.48m + ranked.Score / 20m)
        };
    }

    private static decimal Score(
        PolicyDocumentDto document,
        HashSet<string> reportTerms,
        string normalizedReport,
        string regionCode,
        IssueType reportIssueType)
    {
        if (document.RequiredTermsAny.Count > 0 && !ContainsAny(document.RequiredTermsAny, reportTerms, normalizedReport))
        {
            return 0;
        }

        var keywordHits = document.Keywords.Count(keyword => ContainsSignal(keyword, reportTerms, normalizedReport));
        var domainHits = document.PenaltyUnlessTermsAny.Count(signal => ContainsSignal(signal, reportTerms, normalizedReport));
        if (domainHits == 0)
        {
            return 0;
        }

        var regionBoost = string.Equals(document.RegionCode, regionCode, StringComparison.OrdinalIgnoreCase)
            ? 5
            : string.Equals(document.RegionCode, "regional", StringComparison.OrdinalIgnoreCase) ? 2 : 0;
        var issueBoost = document.IssueTypes.Contains(reportIssueType.ToString(), StringComparer.OrdinalIgnoreCase)
            ? 8
            : 0;

        return keywordHits + domainHits + regionBoost + issueBoost;
    }

    private static string BuildGap(ReportDto report, PolicyDocumentDto document)
    {
        if (string.Equals(report.LanguageCode, "fr", StringComparison.OrdinalIgnoreCase))
        {
            return $"Le temoignage communautaire de {report.Location.Name} indique un ecart possible avec {document.Title}, {document.ArticleReference} : {document.Commitment}";
        }

        return $"Community testimony from {report.Location.Name} raises a possible gap against {document.Title}, {document.ArticleReference}: {document.Commitment}";
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

    private static bool ContainsAny(IReadOnlyList<string> signals, HashSet<string> terms, string normalizedText)
    {
        return signals.Any(signal => ContainsSignal(signal, terms, normalizedText));
    }

    private static bool ContainsSignal(string signal, HashSet<string> terms, string normalizedText)
    {
        var normalizedSignal = Normalize(signal);

        return normalizedSignal.Contains(' ', StringComparison.Ordinal)
            ? normalizedText.Contains(normalizedSignal, StringComparison.Ordinal)
            : terms.Contains(normalizedSignal);
    }

    private static string Normalize(string value)
    {
        return string.Join(' ', value.ToLowerInvariant()
            .Split([' ', ',', '.', ';', ':', '-', '_', '/', '\\', '(', ')'], StringSplitOptions.RemoveEmptyEntries));
    }

}
