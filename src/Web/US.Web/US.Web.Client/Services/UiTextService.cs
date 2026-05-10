using US.SharedKernel.Enums;

namespace US.Web.Client.Services;

public sealed class UiTextService
{
    private static readonly Dictionary<string, Dictionary<string, string>> Translations = new(StringComparer.OrdinalIgnoreCase)
    {
        ["en"] = new(StringComparer.OrdinalIgnoreCase)
        {
            ["Common.Region"] = "Region",
            ["Common.Language"] = "Language",
            ["Common.Location"] = "Location",
            ["Common.IssueType"] = "Issue type",
            ["Common.Status"] = "Status",
            ["Common.Urgency"] = "Urgency",
            ["Common.Submitted"] = "Submitted",
            ["Common.Refresh"] = "Refresh",
            ["Common.Refreshing"] = "Refreshing...",
            ["Common.Online"] = "Online",
            ["Common.Offline"] = "Offline",
            ["Common.Sync"] = "Sync",
            ["Common.Syncing"] = "Syncing...",
            ["Common.Pending"] = "pending",
            ["Submit.Eyebrow"] = "Community reporting",
            ["Submit.Title"] = "Submit a community accountability report",
            ["Submit.Lede"] = "Capture what happened, where it happened, and who is leading the response. AI will help structure the report, but human validators decide what moves forward.",
            ["Submit.LocationPlaceholder"] = "Village, town, landmark, or GPS hint",
            ["Submit.WhatHappened"] = "What happened?",
            ["Submit.DescriptionPlaceholder"] = "Describe the report in the community member's own words.",
            ["Submit.WomenLed"] = "Women-led",
            ["Submit.YouthLed"] = "Youth-led",
            ["Submit.Sensitive"] = "Sensitive/anonymize",
            ["Submit.Submit"] = "Submit report",
            ["Submit.Submitting"] = "Submitting...",
            ["Submit.RegionContext"] = "Region context",
            ["Submit.LoadingRegions"] = "Loading region profiles...",
            ["Submit.RegionalBodies"] = "Regional bodies",
            ["Submit.EscalationPathways"] = "Escalation pathways",
            ["Submit.Submitted"] = "Submitted",
            ["Submit.Queued"] = "Report queued for human validation",
            ["Submit.SavedOffline"] = "No connection to the API right now. The report was saved locally and can be synced later.",
            ["Submit.SyncedAll"] = "Synced {0} pending report(s).",
            ["Submit.SyncedSome"] = "Synced {0} of {1}. {2} still pending.",
            ["Dashboard.Eyebrow"] = "Realtime dashboard",
            ["Dashboard.Title"] = "Community reports as they arrive",
            ["Dashboard.Lede"] = "Validated humans stay in control, while SignalR keeps the dashboard current as reports are submitted or reviewed.",
            ["Dashboard.Live"] = "Live",
            ["Dashboard.Connecting"] = "Connecting",
            ["Dashboard.Total"] = "Total reports",
            ["Dashboard.PendingValidation"] = "Pending validation",
            ["Dashboard.HighCritical"] = "High/Critical urgency",
            ["Dashboard.EmptyTitle"] = "No reports yet",
            ["Dashboard.EmptyBody"] = "Submit a report to see the realtime feed update.",
            ["Dashboard.GenerateBrief"] = "Generate accountability brief",
            ["Validate.Eyebrow"] = "Human validation",
            ["Validate.Title"] = "Review community reports before escalation",
            ["Validate.Lede"] = "AI can structure and summarize evidence, but validators decide what is approved, rejected, or sent for follow-up.",
            ["Validate.Pending"] = "pending validation",
            ["Validate.LoadingTitle"] = "Loading reports",
            ["Validate.LoadingBody"] = "Fetching the latest validation queue.",
            ["Validate.EmptyTitle"] = "No pending reports",
            ["Validate.EmptyBody"] = "New submissions will appear here for human review.",
            ["Validate.Notes"] = "Validator notes",
            ["Validate.NotesPlaceholder"] = "What should advocates or follow-up teams know?",
            ["Validate.Approve"] = "Approve",
            ["Validate.FollowUp"] = "Follow-up",
            ["Validate.Reject"] = "Reject",
            ["Validate.Marked"] = "Report {0} marked as {1}."
        },
        ["fr"] = new(StringComparer.OrdinalIgnoreCase)
        {
            ["Common.Region"] = "Region",
            ["Common.Language"] = "Langue",
            ["Common.Location"] = "Lieu",
            ["Common.IssueType"] = "Type de probleme",
            ["Common.Status"] = "Statut",
            ["Common.Urgency"] = "Urgence",
            ["Common.Submitted"] = "Soumis",
            ["Common.Refresh"] = "Actualiser",
            ["Common.Refreshing"] = "Actualisation...",
            ["Common.Online"] = "En ligne",
            ["Common.Offline"] = "Hors ligne",
            ["Common.Sync"] = "Synchroniser",
            ["Common.Syncing"] = "Synchronisation...",
            ["Common.Pending"] = "en attente",
            ["Submit.Eyebrow"] = "Signalement communautaire",
            ["Submit.Title"] = "Soumettre un rapport de responsabilite communautaire",
            ["Submit.Lede"] = "Documentez ce qui s'est passe, ou cela s'est passe et qui mene la reponse. L'IA aide a structurer le rapport, mais les validateurs humains decident de la suite.",
            ["Submit.LocationPlaceholder"] = "Village, ville, point de repere ou indication GPS",
            ["Submit.WhatHappened"] = "Que s'est-il passe ?",
            ["Submit.DescriptionPlaceholder"] = "Decrivez le rapport avec les mots de la communaute.",
            ["Submit.WomenLed"] = "Dirige par des femmes",
            ["Submit.YouthLed"] = "Dirige par des jeunes",
            ["Submit.Sensitive"] = "Sensible/anonymiser",
            ["Submit.Submit"] = "Soumettre le rapport",
            ["Submit.Submitting"] = "Soumission...",
            ["Submit.RegionContext"] = "Contexte regional",
            ["Submit.LoadingRegions"] = "Chargement des profils regionaux...",
            ["Submit.RegionalBodies"] = "Organismes regionaux",
            ["Submit.EscalationPathways"] = "Voies d'escalade",
            ["Submit.Submitted"] = "Soumis",
            ["Submit.Queued"] = "Rapport en attente de validation humaine",
            ["Submit.SavedOffline"] = "Pas de connexion a l'API pour le moment. Le rapport a ete enregistre localement et pourra etre synchronise plus tard.",
            ["Submit.SyncedAll"] = "{0} rapport(s) en attente synchronise(s).",
            ["Submit.SyncedSome"] = "{0} sur {1} synchronise(s). {2} reste(nt) en attente.",
            ["Dashboard.Eyebrow"] = "Tableau de bord en temps reel",
            ["Dashboard.Title"] = "Les rapports communautaires des leur arrivee",
            ["Dashboard.Lede"] = "Les humains valident et gardent le controle, tandis que SignalR tient le tableau de bord a jour.",
            ["Dashboard.Live"] = "En direct",
            ["Dashboard.Connecting"] = "Connexion",
            ["Dashboard.Total"] = "Total des rapports",
            ["Dashboard.PendingValidation"] = "En attente de validation",
            ["Dashboard.HighCritical"] = "Urgence elevee/critique",
            ["Dashboard.EmptyTitle"] = "Aucun rapport pour le moment",
            ["Dashboard.EmptyBody"] = "Soumettez un rapport pour voir le flux se mettre a jour.",
            ["Dashboard.GenerateBrief"] = "Generer le brief de responsabilite",
            ["Validate.Eyebrow"] = "Validation humaine",
            ["Validate.Title"] = "Examiner les rapports avant escalation",
            ["Validate.Lede"] = "L'IA peut structurer et resumer les preuves, mais les validateurs decident ce qui est approuve, rejete ou renvoye pour suivi.",
            ["Validate.Pending"] = "en attente de validation",
            ["Validate.LoadingTitle"] = "Chargement des rapports",
            ["Validate.LoadingBody"] = "Recuperation de la file de validation.",
            ["Validate.EmptyTitle"] = "Aucun rapport en attente",
            ["Validate.EmptyBody"] = "Les nouvelles soumissions apparaitront ici pour examen humain.",
            ["Validate.Notes"] = "Notes du validateur",
            ["Validate.NotesPlaceholder"] = "Que doivent savoir les defenseurs ou les equipes de suivi ?",
            ["Validate.Approve"] = "Approuver",
            ["Validate.FollowUp"] = "Suivi",
            ["Validate.Reject"] = "Rejeter",
            ["Validate.Marked"] = "Rapport {0} marque comme {1}."
        }
    };

    public string Text(string languageCode, string key)
    {
        var normalized = Normalize(languageCode);
        if (Translations.TryGetValue(normalized, out var language) && language.TryGetValue(key, out var translated))
        {
            return translated;
        }

        return Translations["en"].GetValueOrDefault(key, key);
    }

    public string Format(string languageCode, string key, params object[] args)
    {
        return string.Format(Text(languageCode, key), args);
    }

    public string IssueType(string languageCode, IssueType issueType)
    {
        if (Normalize(languageCode) == "fr")
        {
            return issueType switch
            {
                US.SharedKernel.Enums.IssueType.BrokenPromise => "Promesse non tenue",
                US.SharedKernel.Enums.IssueType.ResourceConflict => "Conflit de ressources",
                US.SharedKernel.Enums.IssueType.GovernanceFailure => "Defaillance de gouvernance",
                US.SharedKernel.Enums.IssueType.SecurityConcern => "Probleme de securite",
                US.SharedKernel.Enums.IssueType.ServiceGap => "Manque de service",
                US.SharedKernel.Enums.IssueType.EnvironmentalHarm => "Prejudice environnemental",
                US.SharedKernel.Enums.IssueType.MediationSuccess => "Mediation reussie",
                US.SharedKernel.Enums.IssueType.RecoveryNeed => "Besoin de retablissement",
                US.SharedKernel.Enums.IssueType.CommunityResilience => "Resilience communautaire",
                _ => "Autre"
            };
        }

        return issueType switch
        {
            US.SharedKernel.Enums.IssueType.BrokenPromise => "Broken promise",
            US.SharedKernel.Enums.IssueType.ResourceConflict => "Resource conflict",
            US.SharedKernel.Enums.IssueType.GovernanceFailure => "Governance failure",
            US.SharedKernel.Enums.IssueType.SecurityConcern => "Security concern",
            US.SharedKernel.Enums.IssueType.ServiceGap => "Service gap",
            US.SharedKernel.Enums.IssueType.EnvironmentalHarm => "Environmental harm",
            US.SharedKernel.Enums.IssueType.MediationSuccess => "Mediation success",
            US.SharedKernel.Enums.IssueType.RecoveryNeed => "Recovery need",
            US.SharedKernel.Enums.IssueType.CommunityResilience => "Community resilience",
            _ => "Other"
        };
    }

    private static string Normalize(string? languageCode)
    {
        return string.Equals(languageCode, "fr", StringComparison.OrdinalIgnoreCase) ? "fr" : "en";
    }
}
