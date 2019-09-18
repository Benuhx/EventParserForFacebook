using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using FacebookEventParser.DTO;

namespace FacebookEventParser.OutputServices {
    internal class HtmlService : IHtmlService {
        public string BaueHtml(List<Verbandsebene> verbaendeMitEvents, List<FacebookPage> geparstePages) {
            var sortierteVeranstaltungen = GetVeranstaltungMitVerband(verbaendeMitEvents);
            var html = GeneriereHtml(sortierteVeranstaltungen, geparstePages);
            return html;
        }

        private string GeneriereHtml(IEnumerable<VeranstaltungMitVerband> sortierteVeranstaltungen, IList<FacebookPage> geparstePages) {
            var sb = new StringBuilder();
            sb.AppendLine(@"<div class=""container"">");
            var rowCounter = 0;
            const string spanBegin = @"<span class=""h4 small"">";
            foreach (var curVeranstaltung in sortierteVeranstaltungen) {
                if (rowCounter % 2 == 0) {
                    sb.AppendLine(@"<div class=""row"">");
                }

                sb.AppendLine("<div class=\"col-sm-6 inner\">");

                var title = ErsetzeLeerstring(curVeranstaltung.Title);
                sb.AppendLine($"<h3>{title}</h3>");

                var datum = ErsetzeLeerstring(FormatiereDatum(curVeranstaltung.ZeitStart));
                sb.AppendLine($"<div class=\"text-left\">{spanBegin}Datum:</span> {datum}</div>");

                var uhrzeit = ErsetzeLeerstring(FormatiereUhrzeit(curVeranstaltung.ZeitStart));
                sb.AppendLine($"<div class=\"text-left\">{spanBegin}Uhrzeit:</span> {uhrzeit}</div>");

                var stadtOrt = BaueStadtOrtString(curVeranstaltung);
                sb.AppendLine($"<div class=\"text-left\">{spanBegin}Ort:</span> {stadtOrt}</div>");

                var veranstalter = ErsetzeLeerstring(curVeranstaltung.Veranstalter.Name);
                sb.AppendLine($"<div class=\"text-left\">{spanBegin}Veranstalter:</span> {veranstalter}</div>");

                var linkTxt = FormatiereHtmlLink("Auf Facebook", curVeranstaltung.Veranstalter.FacebookDesktopUrl);
                sb.AppendLine($"<div class=\"text-left\">{spanBegin}Weitere Infos:</span> {linkTxt}</div>");

                sb.AppendLine("</div>");
                if (rowCounter % 2 == 1) {
                    sb.AppendLine(@"</div>");
                }

                rowCounter++;
            }

            sb.AppendLine(@"</div>");

            var erklaerungsText = BaueErklaerungsText(geparstePages);
            sb.Append("<br>");
            sb.Append($"<div class=\"text-muted small\">{erklaerungsText}</div>");
            var html = sb.ToString();

            return html;
        }

        private IEnumerable<VeranstaltungMitVerband> GetVeranstaltungMitVerband(IEnumerable<Verbandsebene> verbaendeMitEvents) {
            var veranstaltungMitVerband = verbaendeMitEvents
                .Where(x => x.Veranstaltungen != null && x.Veranstaltungen.Count > 0)
                .SelectMany(x => x.Veranstaltungen,
                    (verbandsebene, veranstaltung) => new VeranstaltungMitVerband(veranstaltung, verbandsebene))
                .OrderBy(x => x.ZeitStart)
                .ToList();
            return veranstaltungMitVerband;
        }

        private string BaueErklaerungsText(IList<FacebookPage> geparstePages) {
            var verbaende = geparstePages.Select(x => x.NameDesVerbandes).Take(geparstePages.Count - 1);
            return $"Diese Übersicht fasst die Termine von {string.Join(", ", verbaende)} und {geparstePages.Last().NameDesVerbandes} zusammen.";
        }

        private string FormatiereDatum(DateTime startTime) {
            var deCulture = CultureInfo.GetCultureInfo("de-De");
            return startTime.ToString("dd.MM.yyyy", deCulture);
        }

        private string FormatiereUhrzeit(DateTime curVeranstaltungZeitStart) {
            if (curVeranstaltungZeitStart.Hour == 0 & curVeranstaltungZeitStart.Minute == 0) return string.Empty;

            var deCulture = CultureInfo.GetCultureInfo("de-De");
            const string patternUhrzeit = "HH:mm";

            return curVeranstaltungZeitStart.ToString(patternUhrzeit, deCulture);
        }

        private string BaueDatumUhrzeitString(Veranstaltung veranstaltung) {
            var datumString = ErsetzeLeerstring(FormatiereDatum(veranstaltung.ZeitStart));
            var uhrzeitString = FormatiereUhrzeit(veranstaltung.ZeitStart);

            if (string.IsNullOrWhiteSpace(uhrzeitString)) return datumString;
            return $"{datumString} - {uhrzeitString} Uhr";
        }

        private string BaueStadtOrtString(VeranstaltungMitVerband veranstaltung) {
            var str = string.Empty;

            if (!string.IsNullOrWhiteSpace(veranstaltung.Stadt)) {
                str = veranstaltung.Stadt;
            }

            if (!string.IsNullOrWhiteSpace(veranstaltung.Ort) && !string.IsNullOrWhiteSpace(veranstaltung.Stadt) && veranstaltung.Ort != veranstaltung.Stadt) {
                str = $"{str}, {veranstaltung.Ort}";
            }
            else if (!string.IsNullOrWhiteSpace(veranstaltung.Ort) && string.IsNullOrWhiteSpace(veranstaltung.Stadt)) {
                str = $"{str}{veranstaltung.Ort}";
            }

            return str.Trim();
        }

        private string FormatiereHtmlLink(string linkText, string linkUrl) {
            return $"<a href=\"{linkUrl}\" target=\"_blank\">{linkText}</a>";
        }

        private string ErsetzeLeerstring(string txt) {
            return string.IsNullOrWhiteSpace(txt) ? "Keine Information" : txt;
        }
    }
}