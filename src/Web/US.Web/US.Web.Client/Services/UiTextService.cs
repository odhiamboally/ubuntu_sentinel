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
            ["Common.Ok"] = "OK",
            ["Common.Error"] = "Something went wrong",
            ["Common.View"] = "View",
            ["Common.Edit"] = "Edit",
            ["Common.Delete"] = "Delete",
            ["Common.Cancel"] = "Cancel",
            ["Common.BackToReports"] = "Back to reports",
            ["Nav.Dashboard"] = "Dashboard",
            ["Nav.Reports"] = "Reports",
            ["Nav.AllReports"] = "All reports",
            ["Nav.Submit"] = "Submit",
            ["Nav.Validate"] = "Validate",
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
            ["Submit.SafetyTitle"] = "Human validation required",
            ["Submit.SafetyBody"] = "AI assistance may structure evidence, but community consent, validator review, and safety checks are required before escalation.",
            ["Submit.DescriptionRequired"] = "Describe what happened before submitting.",
            ["Submit.LocationRequired"] = "Add the community location before submitting.",
            ["Submit.DescriptionTooShort"] = "Please provide at least 10 characters so validators have enough context.",
            ["Submit.DescriptionTooLong"] = "Please keep the report under 4,000 characters for this MVP.",
            ["Submit.SubmitFailed"] = "Submission failed. The report was saved locally and can be synced later.",
            ["Submit.Success"] = "Report submitted and queued for human validation.",
            ["Submit.SuccessTitle"] = "Report submitted",
            ["Submit.SuccessDetail"] = "Your report has been queued for human validation.",
            ["Submit.OfflineTitle"] = "Saved for sync",
            ["Submit.SyncTitle"] = "Sync complete",
            ["Submit.ValidationError"] = "Please fix the highlighted issue before submitting.",
            ["Region.drc.Description"] = "Community accountability and reparative justice workflows for mining, displacement, and service-delivery commitments.",
            ["Region.drc.Pathways"] = "Local peace committee -> Provincial authority -> OSF partner review -> Regional body briefing",
            ["Region.sahel.Description"] = "Low-bandwidth civic evidence workflows for communities affected by insecurity, displacement, and governance exclusion.",
            ["Region.sahel.Pathways"] = "Community validator -> Civil society partner -> ECOWAS policy channel",
            ["Region.sudan.Description"] = "Anonymity-aware reporting for communities documenting displacement, local harms, and recovery needs.",
            ["Region.sudan.Pathways"] = "Trusted validator -> Humanitarian partner -> OSF legal or advocacy review",
            ["Region.mozambique.Description"] = "Localized reporting and accountability workflows for recovery, resource governance, and community resilience.",
            ["Region.mozambique.Pathways"] = "Local partner -> District authority -> SADC-aligned advocacy brief",
            ["Dashboard.Eyebrow"] = "Realtime dashboard",
            ["Dashboard.Title"] = "Community accountability metrics",
            ["Dashboard.Lede"] = "Live operational signals for report volume, validation pressure, urgency, follow-up, and approvals.",
            ["Dashboard.Live"] = "Live",
            ["Dashboard.Connecting"] = "Connecting",
            ["Dashboard.Total"] = "Total reports",
            ["Dashboard.PendingValidation"] = "Pending validation",
            ["Dashboard.HighCritical"] = "High/Critical urgency",
            ["Dashboard.FollowUp"] = "Needs follow-up",
            ["Dashboard.Approved"] = "Approved",
            ["Dashboard.EmptyTitle"] = "No reports yet",
            ["Dashboard.EmptyBody"] = "Submit a report to see the realtime feed update.",
            ["Reports.Eyebrow"] = "Report operations",
            ["Reports.Title"] = "Community reports",
            ["Reports.Lede"] = "Browse submitted reports, inspect status, and open accountability briefs without crowding the metrics dashboard.",
            ["Reports.EmptyTitle"] = "No reports yet",
            ["Reports.EmptyBody"] = "Submit a report to populate the operational report list.",
            ["Reports.GenerateBrief"] = "Generate accountability brief",
            ["Reports.GenerateDraftBrief"] = "Generate draft brief",
            ["Reports.EditPendingTitle"] = "Edit report",
            ["Reports.EditPendingBody"] = "Editing is available as a planned workflow after the MVP update endpoint is promoted.",
            ["Reports.DeleteSuccessTitle"] = "Report deleted",
            ["Reports.DeleteSuccess"] = "The report was removed from the operational list.",
            ["Reports.DeleteFailed"] = "The report could not be deleted. Please refresh and try again.",
            ["Reports.DeleteConfirmTitle"] = "Delete report?",
            ["Reports.DeleteConfirmBody"] = "This removes the report from the current MVP data store.",
            ["Validate.Eyebrow"] = "Human validation",
            ["Validate.Title"] = "Review community reports before escalation",
            ["Validate.Lede"] = "AI can structure and summarize evidence, but validators decide what is approved, rejected, or sent for follow-up.",
            ["Validate.Pending"] = "pending validation",
            ["Validate.WorkQueue"] = "in validation/follow-up",
            ["Validate.LoadingTitle"] = "Loading reports",
            ["Validate.LoadingBody"] = "Fetching the latest validation queue.",
            ["Validate.EmptyTitle"] = "No reports need review",
            ["Validate.EmptyBody"] = "New submissions and follow-up cases will appear here.",
            ["Validate.Notes"] = "Validator notes",
            ["Validate.NotesPlaceholder"] = "What should advocates or follow-up teams know?",
            ["Validate.Checklist"] = "Validation checks",
            ["Validate.ConsentCheck"] = "Consent confirmed",
            ["Validate.LocationCheck"] = "Location confidence checked",
            ["Validate.EvidenceCheck"] = "Evidence quality checked",
            ["Validate.SafetyCheck"] = "Reporter safety checked",
            ["Validate.ApproveBlocked"] = "Complete all validation checks before approving.",
            ["Validate.Approve"] = "Approve",
            ["Validate.FollowUp"] = "Follow-up",
            ["Validate.Reject"] = "Reject",
            ["Validate.Marked"] = "Report {0} marked as {1}.",
            ["Validate.ActionCompleteTitle"] = "Validation updated",
            ["Validate.ActionCompleteBody"] = "The report status was updated and the dashboard has been notified.",
            ["Validate.ActionFailed"] = "The validation action could not be completed. Please try again.",
            ["Validate.SensitiveHandling"] = "Sensitive handling required",
            ["Validate.SensitiveDescription"] = "Sensitive report. Validator should confirm consent and safety before exposing full details. Internal description: {0}",
            ["Validate.SafetyTitle"] = "Validator responsibility",
            ["Validate.SafetyBody"] = "Approvals should confirm consent, location confidence, evidence quality, and reporter safety before any institutional escalation."
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
            ["Common.Ok"] = "OK",
            ["Common.Error"] = "Une erreur est survenue",
            ["Common.View"] = "Voir",
            ["Common.Edit"] = "Modifier",
            ["Common.Delete"] = "Supprimer",
            ["Common.Cancel"] = "Annuler",
            ["Common.BackToReports"] = "Retour aux rapports",
            ["Nav.Dashboard"] = "Tableau de bord",
            ["Nav.Reports"] = "Rapports",
            ["Nav.AllReports"] = "Tous les rapports",
            ["Nav.Submit"] = "Soumettre",
            ["Nav.Validate"] = "Valider",
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
            ["Submit.SafetyTitle"] = "Validation humaine requise",
            ["Submit.SafetyBody"] = "L'assistance IA peut structurer les preuves, mais le consentement communautaire, l'examen d'un validateur et les controles de securite sont requis avant toute escalade.",
            ["Submit.DescriptionRequired"] = "Decrivez ce qui s'est passe avant de soumettre.",
            ["Submit.LocationRequired"] = "Ajoutez le lieu communautaire avant de soumettre.",
            ["Submit.DescriptionTooShort"] = "Veuillez fournir au moins 10 caracteres afin que les validateurs aient assez de contexte.",
            ["Submit.DescriptionTooLong"] = "Veuillez garder le rapport sous 4 000 caracteres pour ce MVP.",
            ["Submit.SubmitFailed"] = "La soumission a echoue. Le rapport a ete enregistre localement et pourra etre synchronise plus tard.",
            ["Submit.Success"] = "Rapport soumis et place en attente de validation humaine.",
            ["Submit.SuccessTitle"] = "Rapport soumis",
            ["Submit.SuccessDetail"] = "Votre rapport est en attente de validation humaine.",
            ["Submit.OfflineTitle"] = "Enregistre pour synchronisation",
            ["Submit.SyncTitle"] = "Synchronisation terminee",
            ["Submit.ValidationError"] = "Veuillez corriger le probleme indique avant de soumettre.",
            ["Region.drc.Description"] = "Flux de responsabilite communautaire et de justice reparatrice pour les engagements lies aux mines, aux deplacements et aux services.",
            ["Region.drc.Pathways"] = "Comite local de paix -> Autorite provinciale -> Revue par un partenaire OSF -> Note aux organismes regionaux",
            ["Region.sahel.Description"] = "Flux de preuves civiques a faible bande passante pour les communautes touchees par l'insecurite, le deplacement et l'exclusion de gouvernance.",
            ["Region.sahel.Pathways"] = "Validateur communautaire -> Partenaire de la societe civile -> Canal politique de la CEDEAO",
            ["Region.sudan.Description"] = "Signalement sensible a l'anonymat pour les communautes documentant les deplacements, les prejudices locaux et les besoins de retablissement.",
            ["Region.sudan.Pathways"] = "Validateur de confiance -> Partenaire humanitaire -> Revue juridique ou plaidoyer OSF",
            ["Region.mozambique.Description"] = "Flux localises de signalement et de responsabilite pour le retablissement, la gouvernance des ressources et la resilience communautaire.",
            ["Region.mozambique.Pathways"] = "Partenaire local -> Autorite de district -> Brief de plaidoyer aligne avec la SADC",
            ["Dashboard.Eyebrow"] = "Tableau de bord en temps reel",
            ["Dashboard.Title"] = "Indicateurs de responsabilite communautaire",
            ["Dashboard.Lede"] = "Signaux operationnels en direct sur le volume, la validation, l'urgence, le suivi et les approbations.",
            ["Dashboard.Live"] = "En direct",
            ["Dashboard.Connecting"] = "Connexion",
            ["Dashboard.Total"] = "Total des rapports",
            ["Dashboard.PendingValidation"] = "En attente de validation",
            ["Dashboard.HighCritical"] = "Urgence elevee/critique",
            ["Dashboard.FollowUp"] = "Suivi requis",
            ["Dashboard.Approved"] = "Approuves",
            ["Dashboard.EmptyTitle"] = "Aucun rapport pour le moment",
            ["Dashboard.EmptyBody"] = "Soumettez un rapport pour voir le flux se mettre a jour.",
            ["Reports.Eyebrow"] = "Operations de rapport",
            ["Reports.Title"] = "Rapports communautaires",
            ["Reports.Lede"] = "Consultez les rapports soumis, inspectez les statuts et ouvrez les briefs sans encombrer le tableau de bord.",
            ["Reports.EmptyTitle"] = "Aucun rapport pour le moment",
            ["Reports.EmptyBody"] = "Soumettez un rapport pour remplir la liste operationnelle.",
            ["Reports.GenerateBrief"] = "Generer le brief de responsabilite",
            ["Reports.GenerateDraftBrief"] = "Generer un brouillon",
            ["Reports.EditPendingTitle"] = "Modifier le rapport",
            ["Reports.EditPendingBody"] = "La modification sera disponible apres la promotion du point d'API de mise a jour.",
            ["Reports.DeleteSuccessTitle"] = "Rapport supprime",
            ["Reports.DeleteSuccess"] = "Le rapport a ete retire de la liste operationnelle.",
            ["Reports.DeleteFailed"] = "Le rapport n'a pas pu etre supprime. Veuillez actualiser puis reessayer.",
            ["Reports.DeleteConfirmTitle"] = "Supprimer le rapport ?",
            ["Reports.DeleteConfirmBody"] = "Cette action retire le rapport du stockage MVP actuel.",
            ["Validate.Eyebrow"] = "Validation humaine",
            ["Validate.Title"] = "Examiner les rapports avant escalation",
            ["Validate.Lede"] = "L'IA peut structurer et resumer les preuves, mais les validateurs decident ce qui est approuve, rejete ou renvoye pour suivi.",
            ["Validate.Pending"] = "en attente de validation",
            ["Validate.WorkQueue"] = "en validation/suivi",
            ["Validate.LoadingTitle"] = "Chargement des rapports",
            ["Validate.LoadingBody"] = "Recuperation de la file de validation.",
            ["Validate.EmptyTitle"] = "Aucun rapport a examiner",
            ["Validate.EmptyBody"] = "Les nouvelles soumissions et les cas de suivi apparaitront ici.",
            ["Validate.Notes"] = "Notes du validateur",
            ["Validate.NotesPlaceholder"] = "Que doivent savoir les defenseurs ou les equipes de suivi ?",
            ["Validate.Checklist"] = "Controles de validation",
            ["Validate.ConsentCheck"] = "Consentement confirme",
            ["Validate.LocationCheck"] = "Fiabilite du lieu verifiee",
            ["Validate.EvidenceCheck"] = "Qualite des preuves verifiee",
            ["Validate.SafetyCheck"] = "Securite du declarant verifiee",
            ["Validate.ApproveBlocked"] = "Completez tous les controles avant d'approuver.",
            ["Validate.Approve"] = "Approuver",
            ["Validate.FollowUp"] = "Suivi",
            ["Validate.Reject"] = "Rejeter",
            ["Validate.Marked"] = "Rapport {0} marque comme {1}.",
            ["Validate.ActionCompleteTitle"] = "Validation mise a jour",
            ["Validate.ActionCompleteBody"] = "Le statut du rapport a ete mis a jour et le tableau de bord a ete notifie.",
            ["Validate.ActionFailed"] = "L'action de validation n'a pas pu etre effectuee. Veuillez reessayer.",
            ["Validate.SensitiveHandling"] = "Traitement sensible requis",
            ["Validate.SensitiveDescription"] = "Rapport sensible. Le validateur doit confirmer le consentement et la securite avant d'exposer les details. Description interne : {0}",
            ["Validate.SafetyTitle"] = "Responsabilite du validateur",
            ["Validate.SafetyBody"] = "Les approbations doivent confirmer le consentement, la fiabilite du lieu, la qualite des preuves et la securite du declarant avant toute escalade institutionnelle."
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

    public string Status(string languageCode, ReportStatus status)
    {
        if (Normalize(languageCode) == "fr")
        {
            return status switch
            {
                ReportStatus.Draft => "Brouillon",
                ReportStatus.QueuedOffline => "En attente hors ligne",
                ReportStatus.Submitted => "Soumis",
                ReportStatus.Processing => "En traitement",
                ReportStatus.PendingValidation => "En validation",
                ReportStatus.Approved => "Approuve",
                ReportStatus.NeedsFollowUp => "Suivi requis",
                ReportStatus.Rejected => "Rejete",
                _ => status.ToString()
            };
        }

        return status switch
        {
            ReportStatus.Draft => "Draft",
            ReportStatus.QueuedOffline => "Queued offline",
            ReportStatus.Submitted => "Submitted",
            ReportStatus.Processing => "Processing",
            ReportStatus.PendingValidation => "Pending validation",
            ReportStatus.Approved => "Approved",
            ReportStatus.NeedsFollowUp => "Needs follow-up",
            ReportStatus.Rejected => "Rejected",
            _ => status.ToString()
        };
    }

    public string Urgency(string languageCode, UrgencyLevel urgency)
    {
        if (Normalize(languageCode) == "fr")
        {
            return urgency switch
            {
                UrgencyLevel.Low => "Faible",
                UrgencyLevel.Medium => "Moyenne",
                UrgencyLevel.High => "Elevee",
                UrgencyLevel.Critical => "Critique",
                _ => urgency.ToString()
            };
        }

        return urgency switch
        {
            UrgencyLevel.Low => "Low",
            UrgencyLevel.Medium => "Medium",
            UrgencyLevel.High => "High",
            UrgencyLevel.Critical => "Critical",
            _ => urgency.ToString()
        };
    }

    private static string Normalize(string? languageCode)
    {
        return string.Equals(languageCode, "fr", StringComparison.OrdinalIgnoreCase) ? "fr" : "en";
    }
}
