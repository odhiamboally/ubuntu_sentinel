using System.Globalization;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using US.SharedKernel.Contracts.Reports;
using US.SharedKernel.Enums;

namespace US.Api.Features.Accountability;

public interface IBriefPdfRenderer
{
    byte[] Render(ReportDto report, ReportPipelineResultDto result);
}

public sealed class QuestPdfBriefRenderer : IBriefPdfRenderer
{
    public byte[] Render(ReportDto report, ReportPipelineResultDto result)
    {
        return new AccountabilityBriefDocument(report, result).GeneratePdf();
    }

    private sealed class AccountabilityBriefDocument(ReportDto report, ReportPipelineResultDto result) : IDocument
    {
        private readonly DateTimeOffset _generatedAt = DateTimeOffset.UtcNow;
        private readonly bool _isFrench = string.Equals(report.LanguageCode, "fr", StringComparison.OrdinalIgnoreCase);
        private readonly bool _isValidationBacked = report.Status == ReportStatus.Approved && report.ValidationChecks.IsComplete;

        public DocumentMetadata GetMetadata() => new()
        {
            Title = $"Ubuntu Sentinel brief - {report.Location.Name}",
            Author = "Ubuntu Sentinel",
            Subject = "Accountability brief",
            Creator = "Ubuntu Sentinel QuestPDF export",
            Keywords = "Ubuntu Sentinel, accountability, community report, brief"
        };

        public void Compose(IDocumentContainer container)
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(28);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(text => text.FontSize(10).FontColor(Palette.Ink));

                page.Header().Element(ComposeHeader);
                page.Content().PaddingTop(16).Column(column =>
                {
                    column.Spacing(14);
                    column.Item().Element(ComposeSafetyBanner);
                    column.Item().Element(ComposeOverview);

                    if (!string.IsNullOrWhiteSpace(result.FallbackReason))
                    {
                        column.Item().Element(ComposeFallbackBanner);
                    }

                    column.Item().Element(ComposeNarrativeBrief);
                    column.Item().Element(ComposeValidationSection);
                    column.Item().Element(ComposeTraceabilitySection);
                });
                page.Footer().Element(ComposeFooter);
            });
        }

        private void ComposeHeader(IContainer container)
        {
            container.Row(row =>
            {
                row.RelativeItem().Column(column =>
                {
                    column.Spacing(4);
                    column.Item().Text("Ubuntu Sentinel").Bold().FontSize(18).FontColor(Palette.Brand);
                    column.Item().Text(Localize("Accountability brief", "Brief de responsabilite"))
                        .SemiBold()
                        .FontSize(20);
                    column.Item().Text(Localize(
                            "Community-led evidence, validation, and accountability output.",
                            "Sortie de responsabilite fondee sur la preuve communautaire et la validation humaine."))
                        .FontColor(Palette.Muted);
                });

                row.ConstantItem(180).AlignMiddle().Element(container =>
                {
                    container.AlignRight().Column(column =>
                    {
                        column.Spacing(6);
                        column.Item().AlignRight().Element(c => ComposeBadge(
                            c,
                            GetHeaderBadgeLabel(),
                            GetHeaderBadgeBackground(),
                            Colors.White));
                        column.Item().AlignRight().Text(Localize("Generated", "Genere"))
                            .SemiBold()
                            .FontSize(8)
                            .FontColor(Palette.Muted);
                        column.Item().AlignRight().Text(FormatDateTime(_generatedAt))
                            .FontSize(9);
                    });
                });
            });
        }

        private void ComposeSafetyBanner(IContainer container)
        {
            var background = _isValidationBacked ? Palette.SuccessSurface : Palette.WarningSurface;
            var accent = _isValidationBacked ? Palette.Success : Palette.Warning;

            container.Border(1)
                .BorderColor(accent)
                .Background(background)
                .Padding(12)
                .Column(column =>
                {
                    column.Spacing(6);
                    column.Item().Text(_isValidationBacked
                            ? Localize("Validation-backed brief", "Brief valide")
                            : Localize("Internal draft / human validation required", "Brouillon interne / validation humaine requise"))
                        .SemiBold()
                        .FontColor(accent);
                    column.Item().Text(_isValidationBacked
                            ? Localize(
                                "This brief can support responsible partner review because consent, location, evidence, and reporter safety checks were completed.",
                                "Ce brief peut soutenir une revue responsable parce que les controles de consentement, de lieu, de preuve et de securite ont ete completes.")
                            : Localize(
                                "This PDF is structured assistance only. It does not authorize escalation until a validator completes every required check.",
                                "Ce PDF reste une assistance structuree. Il n'autorise aucune escalation tant qu'un validateur n'a pas complete tous les controles requis."))
                        .LineHeight(1.35f);
                });
        }

        private void ComposeOverview(IContainer container)
        {
            container.Column(column =>
            {
                column.Spacing(8);
                column.Item().Row(row =>
                {
                    row.Spacing(8);
                    row.RelativeItem().Element(c => ComposeFactCard(c, Localize("Community", "Communaute"), report.Location.Name));
                    row.RelativeItem().Element(c => ComposeFactCard(c, Localize("Region", "Region"), report.RegionCode.ToUpperInvariant()));
                });
                column.Item().Row(row =>
                {
                    row.Spacing(8);
                    row.RelativeItem().Element(c => ComposeFactCard(c, Localize("Issue type", "Type de probleme"), FormatIssueType(report.IssueType)));
                    row.RelativeItem().Element(c => ComposeFactCard(c, Localize("Urgency", "Urgence"), FormatUrgency(result.Urgency)));
                });
                column.Item().Row(row =>
                {
                    row.Spacing(8);
                    row.RelativeItem().Element(c => ComposeFactCard(c, Localize("Status", "Statut"), FormatStatus(report.Status)));
                    row.RelativeItem().Element(c => ComposeFactCard(c, Localize("Validation gate", "Gate de validation"), GetValidationGateLabel()));
                });
                column.Item().Row(row =>
                {
                    row.Spacing(8);
                    row.RelativeItem().Element(c => ComposeFactCard(c, Localize("Pipeline mode", "Mode du pipeline"), FormatPipelineMode()));
                    row.RelativeItem().Element(c => ComposeFactCard(c, Localize("Confidence", "Confiance"), result.Confidence.ToString("0.00", CultureInfo.InvariantCulture)));
                });
            });
        }

        private void ComposeFallbackBanner(IContainer container)
        {
            container.Border(1)
                .BorderColor(Palette.Border)
                .Background(Palette.SurfaceStrong)
                .Padding(12)
                .Column(column =>
                {
                    column.Spacing(4);
                    column.Item().Text(Localize("Pipeline note", "Note du pipeline"))
                        .SemiBold()
                        .FontColor(Palette.Brand);
                    column.Item().Text(result.FallbackReason!).LineHeight(1.3f);
                });
        }

        private void ComposeNarrativeBrief(IContainer container)
        {
            var blocks = PrepareBlocks(result.AccountabilityBriefMarkdown);

            ComposeSection(container, Localize("Structured brief", "Brief structure"), column =>
            {
                foreach (var block in blocks)
                {
                    switch (block.Kind)
                    {
                        case BriefBlockKind.Section:
                            column.Item().PaddingTop(4).Text(block.Text)
                                .FontSize(14)
                                .SemiBold()
                                .FontColor(Palette.Brand);
                            break;
                        case BriefBlockKind.Paragraph:
                            column.Item()
                                .DefaultTextStyle(TextStyle.Default.LineHeight(1.35f))
                                .Text(text => ComposeInline(text, block.Text));
                            break;
                        case BriefBlockKind.Bullet:
                            column.Item().Row(row =>
                            {
                                row.Spacing(6);
                                row.ConstantItem(12).AlignTop().Text("•").Bold().FontColor(Palette.Brand);
                                row.RelativeItem()
                                    .DefaultTextStyle(TextStyle.Default.LineHeight(1.3f))
                                    .Text(text => ComposeInline(text, block.Text));
                            });
                            break;
                        case BriefBlockKind.OrderedItem:
                            column.Item().Row(row =>
                            {
                                row.Spacing(6);
                                row.ConstantItem(18).AlignTop().Text($"{block.Order}.").Bold().FontColor(Palette.Brand);
                                row.RelativeItem()
                                    .DefaultTextStyle(TextStyle.Default.LineHeight(1.3f))
                                    .Text(text => ComposeInline(text, block.Text));
                            });
                            break;
                        case BriefBlockKind.Note:
                            column.Item()
                                .Background(Palette.SurfaceStrong)
                                .Padding(10)
                                .DefaultTextStyle(TextStyle.Default.Italic().FontColor(Palette.Muted))
                                .Text(text => ComposeInline(text, block.Text));
                            break;
                        case BriefBlockKind.Rule:
                            column.Item().LineHorizontal(1).LineColor(Palette.Border);
                            break;
                        case BriefBlockKind.Empty:
                            column.Item().Height(4);
                            break;
                    }
                }
            });
        }

        private void ComposeValidationSection(IContainer container)
        {
            ComposeSection(container, Localize("Validation and safety", "Validation et securite"), column =>
            {
                column.Item().Text(_isValidationBacked
                        ? Localize(
                            "Human validation is complete for consent, location confidence, evidence quality, and reporter safety.",
                            "La validation humaine est complete pour le consentement, la fiabilite du lieu, la qualite des preuves et la securite du declarant.")
                        : Localize(
                            "Keep this output internal until every validation check below is complete.",
                            "Conservez cette sortie en interne jusqu'a ce que chaque controle ci-dessous soit complete."))
                    .LineHeight(1.3f);

                column.Item().Element(c => ComposeChecklistItem(c, Localize("Consent confirmed", "Consentement confirme"), report.ValidationChecks.ConsentConfirmed));
                column.Item().Element(c => ComposeChecklistItem(c, Localize("Location confidence checked", "Fiabilite du lieu verifiee"), report.ValidationChecks.LocationConfidenceChecked));
                column.Item().Element(c => ComposeChecklistItem(c, Localize("Evidence quality checked", "Qualite des preuves verifiee"), report.ValidationChecks.EvidenceQualityChecked));
                column.Item().Element(c => ComposeChecklistItem(c, Localize("Reporter safety checked", "Securite du declarant verifiee"), report.ValidationChecks.ReporterSafetyChecked));
                column.Item().Element(c => ComposeChecklistItem(c, Localize("Sensitive handling required", "Traitement sensible requis"), report.IsSensitive, highlightWhenFalse: false));

                column.Item().PaddingTop(4).Text(Localize("Validator notes", "Notes du validateur"))
                    .SemiBold()
                    .FontColor(Palette.Brand);
                column.Item().Background(Palette.SurfaceStrong)
                    .Padding(10)
                    .Text(string.IsNullOrWhiteSpace(report.ValidatorNotes)
                        ? Localize("No validator notes recorded yet.", "Aucune note du validateur n'est encore enregistree.")
                        : report.ValidatorNotes)
                    .LineHeight(1.3f);
            });
        }

        private void ComposeTraceabilitySection(IContainer container)
        {
            ComposeSection(container, Localize("Traceability", "Tracabilite"), column =>
            {
                column.Item().Text(Localize("Policy source", "Source politique"))
                    .SemiBold()
                    .FontColor(Palette.Brand);
                column.Item().Background(Palette.SurfaceStrong).Padding(10).Column(sourceColumn =>
                {
                    sourceColumn.Spacing(4);
                    sourceColumn.Item().Text(result.PolicyMatch is null
                        ? Localize("No confident source clause was attached yet.", "Aucune clause source fiable n'a encore ete jointe.")
                        : result.PolicyMatch.DocumentTitle).SemiBold();
                    sourceColumn.Item().Text(result.PolicyMatch is null
                        ? Localize("A validator should attach the relevant commitment or rights source before escalation.", "Un validateur doit joindre l'engagement ou la source de droits pertinente avant toute escalation.")
                        : $"{result.PolicyMatch.ArticleReference} · {result.PolicyMatch.DocumentType} · {result.PolicyMatch.Similarity:0.00}")
                        .FontColor(Palette.Muted);

                    if (result.PolicyMatch is not null)
                    {
                        sourceColumn.Item().Text(result.PolicyMatch.Commitment).LineHeight(1.3f);
                        sourceColumn.Item().Text($"{Localize("Source", "Source")}: {result.PolicyMatch.Source} · {result.PolicyMatch.SourceUrl}")
                            .FontSize(9)
                            .FontColor(Palette.Muted);
                    }
                });

                column.Item().Text(Localize("Pipeline signals", "Signaux du pipeline"))
                    .SemiBold()
                    .FontColor(Palette.Brand);
                column.Item().Background(Palette.SurfaceStrong).Padding(10).Column(signalColumn =>
                {
                    signalColumn.Spacing(4);
                    signalColumn.Item().Text($"{Localize("Mode", "Mode")}: {FormatPipelineMode()}");

                    if (!string.IsNullOrWhiteSpace(result.PipelineModel))
                    {
                        signalColumn.Item().Text($"{Localize("Model", "Modele")}: {result.PipelineModel}");
                    }

                    signalColumn.Item().Text($"{Localize("Flags", "Drapeaux")}: {FormatList(result.Flags, Localize("None", "Aucun"))}");
                    signalColumn.Item().Text($"{Localize("Citations", "Citations")}: {FormatList(result.Citations, Localize("None", "Aucune"))}")
                        .LineHeight(1.3f);
                });
            });
        }

        private void ComposeFooter(IContainer container)
        {
            container.PaddingTop(8).BorderTop(1).BorderColor(Palette.Border).Row(row =>
            {
                row.RelativeItem().Text(Localize(
                        "Ubuntu Sentinel keeps original community testimony and requires human validation before escalation.",
                        "Ubuntu Sentinel preserve le temoignage communautaire original et exige une validation humaine avant toute escalation."))
                    .FontSize(8)
                    .FontColor(Palette.Muted);
                row.ConstantItem(110)
                    .AlignRight()
                    .DefaultTextStyle(TextStyle.Default.FontSize(8).FontColor(Palette.Muted))
                    .Text(text =>
                    {
                        text.Span(Localize("Page ", "Page "));
                        text.CurrentPageNumber();
                        text.Span(Localize(" of ", " sur "));
                        text.TotalPages();
                    });
            });
        }

        private void ComposeSection(IContainer container, string title, Action<ColumnDescriptor> buildContent)
        {
            container.Border(1)
                .BorderColor(Palette.Border)
                .Background(Colors.White)
                .Padding(14)
                .Column(column =>
                {
                    column.Spacing(8);
                    column.Item().Text(title).FontSize(15).SemiBold().FontColor(Palette.Brand);
                    column.Item().LineHorizontal(1).LineColor(Palette.Border);
                    buildContent(column);
                });
        }

        private void ComposeFactCard(IContainer container, string label, string value)
        {
            container.Border(1)
                .BorderColor(Palette.Border)
                .Background(Palette.Surface)
                .Padding(12)
                .Column(column =>
                {
                    column.Spacing(4);
                    column.Item().Text(label).FontSize(8).SemiBold().FontColor(Palette.Muted);
                    column.Item().Text(value).FontSize(11).SemiBold().FontColor(Palette.Ink);
                });
        }

        private void ComposeBadge(IContainer container, string text, string backgroundColor, string textColor)
        {
            container.Background(backgroundColor)
                .PaddingVertical(6)
                .PaddingHorizontal(10)
                .Text(text)
                .FontSize(8)
                .SemiBold()
                .FontColor(textColor)
                .AlignCenter();
        }

        private void ComposeChecklistItem(IContainer container, string label, bool done, bool highlightWhenFalse = true)
        {
            var color = done
                ? Palette.Success
                : highlightWhenFalse ? Palette.Warning : Palette.Muted;

            container.Row(row =>
            {
                row.Spacing(8);
                row.ConstantItem(16).AlignMiddle().Text(done ? "●" : "○").FontColor(color).FontSize(11);
                row.RelativeItem().Text($"{label}: {Localize(done ? "Yes" : "No", done ? "Oui" : "Non")}");
            });
        }

        private string Localize(string english, string french) => _isFrench ? french : english;

        private string GetHeaderBadgeLabel()
        {
            if (report.Status == ReportStatus.Rejected)
            {
                return Localize("Internal rejection", "Rejet interne");
            }

            if (report.Status == ReportStatus.NeedsFollowUp)
            {
                return Localize("Follow-up required", "Suivi requis");
            }

            return _isValidationBacked
                ? Localize("Validation-backed", "Valide")
                : Localize("Internal draft", "Brouillon interne");
        }

        private string GetHeaderBadgeBackground()
        {
            if (report.Status == ReportStatus.Rejected)
            {
                return Palette.Danger;
            }

            if (report.Status == ReportStatus.NeedsFollowUp)
            {
                return Palette.Warning;
            }

            return _isValidationBacked ? Palette.Success : Palette.Brand;
        }

        private string GetValidationGateLabel() => _isValidationBacked
            ? Localize("Complete", "Complet")
            : Localize("Not escalation-ready", "Pas pret pour escalation");

        private string FormatPipelineMode() => result.PipelineMode switch
        {
            "OpenAI-backed" => Localize("OpenAI-backed", "Appuye par OpenAI"),
            "Deterministic fallback" => Localize("Deterministic fallback", "Secours deterministe"),
            _ => result.PipelineMode
        };

        private string FormatIssueType(IssueType issueType) => (_isFrench, issueType) switch
        {
            (true, IssueType.BrokenPromise) => "Promesse non tenue",
            (true, IssueType.ResourceConflict) => "Conflit de ressources",
            (true, IssueType.GovernanceFailure) => "Defaillance de gouvernance",
            (true, IssueType.SecurityConcern) => "Probleme de securite",
            (true, IssueType.ServiceGap) => "Manque de service",
            (true, IssueType.EnvironmentalHarm) => "Prejudice environnemental",
            (true, IssueType.MediationSuccess) => "Succes de mediation",
            (true, IssueType.RecoveryNeed) => "Besoin de retablissement",
            (true, IssueType.CommunityResilience) => "Resilience communautaire",
            (true, _) => "Autre",
            (false, _) => issueType switch
            {
                IssueType.BrokenPromise => "Broken promise",
                IssueType.ResourceConflict => "Resource conflict",
                IssueType.GovernanceFailure => "Governance failure",
                IssueType.SecurityConcern => "Security concern",
                IssueType.ServiceGap => "Service gap",
                IssueType.EnvironmentalHarm => "Environmental harm",
                IssueType.MediationSuccess => "Mediation success",
                IssueType.RecoveryNeed => "Recovery need",
                IssueType.CommunityResilience => "Community resilience",
                _ => "Other"
            }
        };

        private string FormatUrgency(UrgencyLevel urgency) => (_isFrench, urgency) switch
        {
            (true, UrgencyLevel.Low) => "Faible",
            (true, UrgencyLevel.Medium) => "Moyenne",
            (true, UrgencyLevel.High) => "Elevee",
            (true, UrgencyLevel.Critical) => "Critique",
            _ => urgency.ToString()
        };

        private string FormatStatus(ReportStatus status) => (_isFrench, status) switch
        {
            (true, ReportStatus.Draft) => "Brouillon",
            (true, ReportStatus.QueuedOffline) => "Hors ligne",
            (true, ReportStatus.Submitted) => "Soumis",
            (true, ReportStatus.Processing) => "Traitement",
            (true, ReportStatus.PendingValidation) => "En attente de validation",
            (true, ReportStatus.NeedsFollowUp) => "Suivi requis",
            (true, ReportStatus.Approved) => "Approuve",
            (true, ReportStatus.Rejected) => "Rejete",
            _ => status switch
            {
                ReportStatus.QueuedOffline => "Queued offline",
                ReportStatus.Processing => "Processing",
                ReportStatus.PendingValidation => "Pending validation",
                ReportStatus.NeedsFollowUp => "Needs follow-up",
                _ => status.ToString()
            }
        };

        private static string FormatList(IReadOnlyList<string> values, string emptyValue)
        {
            return values.Count == 0 ? emptyValue : string.Join("; ", values);
        }

        private string FormatDateTime(DateTimeOffset value)
        {
            var culture = _isFrench ? CultureInfo.GetCultureInfo("fr-FR") : CultureInfo.GetCultureInfo("en-US");
            return value.ToString("dd MMM yyyy HH:mm 'UTC'", culture);
        }

        private static IReadOnlyList<BriefBlock> PrepareBlocks(string markdown)
        {
            var blocks = ParseBrief(markdown).ToList();

            if (blocks.Count > 0 && blocks[0].Kind == BriefBlockKind.Title)
            {
                blocks.RemoveAt(0);
            }

            while (blocks.Count > 0 && blocks[0].Kind == BriefBlockKind.Empty)
            {
                blocks.RemoveAt(0);
            }

            while (blocks.Count > 0 &&
                   blocks[0].Kind == BriefBlockKind.Paragraph &&
                   blocks[0].Text.StartsWith("**", StringComparison.Ordinal))
            {
                blocks.RemoveAt(0);

                while (blocks.Count > 0 && blocks[0].Kind == BriefBlockKind.Empty)
                {
                    blocks.RemoveAt(0);
                }
            }

            return blocks;
        }

        private static IReadOnlyList<BriefBlock> ParseBrief(string markdown)
        {
            if (string.IsNullOrWhiteSpace(markdown))
            {
                return [];
            }

            return markdown.Replace("\r\n", "\n").Split('\n')
                .Select(line =>
                {
                    var trimmed = line.Trim();
                    if (string.IsNullOrWhiteSpace(trimmed))
                    {
                        return new BriefBlock(BriefBlockKind.Empty, string.Empty);
                    }

                    if (trimmed == "---")
                    {
                        return new BriefBlock(BriefBlockKind.Rule, string.Empty);
                    }

                    if (trimmed.StartsWith("# ", StringComparison.Ordinal))
                    {
                        return new BriefBlock(BriefBlockKind.Title, trimmed[2..].Trim());
                    }

                    if (trimmed.StartsWith("## ", StringComparison.Ordinal))
                    {
                        return new BriefBlock(BriefBlockKind.Section, trimmed[3..].Trim());
                    }

                    if (trimmed.StartsWith("- ", StringComparison.Ordinal))
                    {
                        return new BriefBlock(BriefBlockKind.Bullet, trimmed[2..].Trim());
                    }

                    if (TryParseOrderedItem(trimmed, out var order, out var orderedText))
                    {
                        return new BriefBlock(BriefBlockKind.OrderedItem, orderedText, order);
                    }

                    if (trimmed.StartsWith("*", StringComparison.Ordinal) && trimmed.EndsWith("*", StringComparison.Ordinal))
                    {
                        return new BriefBlock(BriefBlockKind.Note, trimmed.Trim('*').Trim());
                    }

                    return new BriefBlock(BriefBlockKind.Paragraph, trimmed);
                })
                .ToArray();
        }

        private static bool TryParseOrderedItem(string value, out int order, out string text)
        {
            order = 0;
            text = string.Empty;

            var separatorIndex = value.IndexOf(". ", StringComparison.Ordinal);
            if (separatorIndex <= 0 || !int.TryParse(value[..separatorIndex], out order))
            {
                return false;
            }

            text = value[(separatorIndex + 2)..].Trim();
            return !string.IsNullOrWhiteSpace(text);
        }

        private static void ComposeInline(TextDescriptor text, string markdown)
        {
            var segments = markdown.Split("**");
            for (var index = 0; index < segments.Length; index++)
            {
                var span = text.Span(segments[index]);
                if (index % 2 == 1)
                {
                    span.SemiBold();
                }
            }
        }

        private sealed record BriefBlock(BriefBlockKind Kind, string Text, int Order = 0);

        private enum BriefBlockKind
        {
            Empty,
            Title,
            Section,
            Paragraph,
            Bullet,
            OrderedItem,
            Note,
            Rule
        }

        private static class Palette
        {
            public const string Brand = "#9B2C2C";
            public const string Ink = "#1F2937";
            public const string Muted = "#667085";
            public const string Border = "#D0D5DD";
            public const string Surface = "#F8FAFC";
            public const string SurfaceStrong = "#F2F4F7";
            public const string Success = "#157F3B";
            public const string SuccessSurface = "#ECFDF3";
            public const string Warning = "#B54708";
            public const string WarningSurface = "#FFF7ED";
            public const string Danger = "#B42318";
        }
    }
}
