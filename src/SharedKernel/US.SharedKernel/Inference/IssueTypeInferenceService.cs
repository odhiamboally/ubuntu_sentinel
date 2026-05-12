using US.SharedKernel.Contracts.Reports;
using US.SharedKernel.Enums;

namespace US.SharedKernel.Inference;

public sealed class IssueTypeInferenceService
{
    private static readonly IReadOnlyDictionary<IssueType, string[]> Signals = new Dictionary<IssueType, string[]>
    {
        [IssueType.SecurityConcern] =
        [
            "police", "army", "soldier", "arrest", "shot", "killed", "beaten",
            "goon", "goons", "militia", "attacked", "threatened", "detained", "tortured",
            "intimidat", "violence", "weapon", "gun", "forced",
            "armee", "soldat", "soldats", "arrete", "arreter", "arrestation", "tue",
            "blesse", "battu", "milice", "attaque", "menace", "menaces", "detenu",
            "detention", "torture", "arme", "fusil", "force", "harcele", "harcelement",
            "abus", "securite"
        ],
        [IssueType.ResourceConflict] =
        [
            "mine", "mining", "company", "extraction", "oil", "gas", "drill",
            "land taken", "evict", "resource", "water point", "grazing", "boundary",
            "mines", "societe miniere", "compagnie", "compagnie miniere", "petrole",
            "gaz", "forage", "terre prise", "expulse", "ressource", "ressources",
            "point d'eau", "paturage", "frontiere", "limite", "terrain"
        ],
        [IssueType.EnvironmentalHarm] =
        [
            "river", "water", "forest", "burn", "burning", "pollution", "toxic", "chemical",
            "catchment", "soil", "fish", "dying", "contaminate", "waste",
            "disease", "sick", "black water", "dead animals", "smoke",
            "riviere", "eau", "foret", "brule", "bruler", "incendie", "pollue",
            "pollution", "toxique", "chimique", "bassin versant", "sol", "poissons",
            "meurent", "contamine", "dechets", "maladie", "malades", "eau noire",
            "animaux morts", "fumee"
        ],
        [IssueType.ServiceGap] =
        [
            "health insurance", "insurance", "salary", "salaries", "pay for it", "paying for it",
            "deducted", "deduction", "benefits", "benefit not working", "not working",
            "service not working", "clinic closed", "hospital not working", "medicine unavailable",
            "assurance sante", "assurance", "salaire", "salaires", "paie", "payer",
            "payons", "deduit", "deduction", "retenue", "avantages", "service ne fonctionne pas",
            "ne fonctionne pas", "centre de sante", "clinique fermee", "hopital ne fonctionne pas",
            "medicaments indisponibles", "frais", "soins"
        ],
        [IssueType.BrokenPromise] =
        [
            "school", "hospital", "clinic", "road", "promised", "never built",
            "not built", "not delivered", "fund", "payment", "quarter",
            "months ago", "years ago", "agreement", "commitment", "cda", "benefit",
            "ecole", "hopital", "clinique", "route", "promis", "promise", "jamais construit",
            "pas construit", "pas livre", "fonds", "paiement", "trimestre", "mois",
            "annees", "accord", "engagement", "benefice"
        ],
        [IssueType.RecoveryNeed] =
        [
            "displaced", "fled", "camp", "refugee", "idp", "shelter", "food",
            "aid", "blocked", "cannot return", "evicted", "homeless", "forced to leave",
            "deplace", "deplaces", "fui", "camp", "refugie", "refugies", "abri",
            "nourriture", "aide", "bloque", "bloquee", "ne peut pas rentrer",
            "sans abri", "force de partir", "expulse"
        ],
        [IssueType.GovernanceFailure] =
        [
            "stolen", "diverted", "corrupt", "bribe", "embezzl", "misuse",
            "disappeared", "not accounted", "official", "councillor", "pocketed", "procurement",
            "government", "authority", "authorities", "demolition", "demolished", "market",
            "without notice", "no notice", "notice", "permit", "licence", "license",
            "vole", "detourne", "corrompu", "corruption", "pot de vin", "abus de fonds",
            "disparu", "non justifie", "officiel", "conseiller", "gouvernement",
            "autorite", "autorites", "demolition", "demoli", "marche", "sans preavis",
            "sans avis", "preavis", "permis", "licence", "marches publics"
        ],
        [IssueType.MediationSuccess] =
        [
            "mediated", "mediation", "agreement reached", "resolved", "elders", "women led mediation",
            "peace committee", "reconciled",
            "mediation", "accord conclu", "resolu", "anciens", "sages",
            "mediation des femmes", "comite de paix", "reconcilie"
        ],
        [IssueType.CommunityResilience] =
        [
            "early warning", "youth network", "safe route", "community patrol", "shared market",
            "resilience", "peace story", "cooperation",
            "alerte precoce", "reseau de jeunes", "route sure", "patrouille communautaire",
            "marche partage", "histoire de paix", "cooperation"
        ]
    };

    public IssueTypeInferenceResultDto Infer(string? description)
    {
        if (string.IsNullOrWhiteSpace(description))
        {
            return IssueTypeInferenceResultDto.Uncertain();
        }

        var lower = description.ToLowerInvariant();
        var scores = new Dictionary<IssueType, List<string>>();

        foreach (var (issueType, keywords) in Signals)
        {
            var hits = keywords
                .Where(keyword => lower.Contains(keyword, StringComparison.OrdinalIgnoreCase))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            if (hits.Count > 0)
            {
                scores[issueType] = hits;
            }
        }

        if (scores.Count == 0)
        {
            return IssueTypeInferenceResultDto.Uncertain();
        }

        var top = scores
            .OrderByDescending(item => item.Value.Count)
            .ThenBy(item => item.Key)
            .First();
        var total = scores.Values.Sum(items => items.Count);
        var confidence = Math.Clamp((decimal)top.Value.Count / (total + 2), 0.1m, 0.95m);

        if (top.Value.Count >= 3)
        {
            confidence = Math.Min(0.95m, confidence + 0.2m);
        }

        return new IssueTypeInferenceResultDto
        {
            SuggestedType = top.Key,
            Confidence = confidence,
            Signals = top.Value,
            Source = InferenceSource.Deterministic
        };
    }
}
