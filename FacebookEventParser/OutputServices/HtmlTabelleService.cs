﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using FacebookEventParser.DTO;

namespace FacebookEventParser.OutputServices {
    public interface IHtmlService {
        string BaueHtml(List<Verbandsebene> verbaendeMitEvents, List<FacebookPage> geparstePages);
    }

    public class HtmlTabelleService : IHtmlService {
        public string BaueHtml(List<Verbandsebene> verbaendeMitEvents, List<FacebookPage> geparstePages) {
            var sortierteVeranstaltungen = GetVeranstaltungMitVerband(verbaendeMitEvents);
            var html = GeneriereHtmlTabelle(sortierteVeranstaltungen, geparstePages);
            return html;
        }

        private string GeneriereHtmlTabelle(IEnumerable<VeranstaltungMitVerband> sortierteVeranstaltungen, IList<FacebookPage> geparstePages) {
            var stringBuilderHtmlTabelle = new StringBuilder();
            // stringBuilderHtmlTabelle.Append("<style>\r\n#JuLiEventsGen {\r\n    font-family: Merriweather, Arial, Helvetica, sans-serif;\r\n    border-collapse: collapse;\r\n    width: 100%;\r\n}\r\n\r\n#JuLiEventsGen thead {\r\n    font-family: Montserrat, Arial, Helvetica, sans-serif;\r\n    font-weight: bold;\r\n    background-color: rgb(255, 237, 0);\r\n    color: rgb(229, 0, 125);\r\n}\r\n\r\n#JuLiEventsGen td, #JuLiEventsGen th {\r\n    border: 1px solid rgb(255, 237, 0);\r\n    padding: 8px;\r\n}\r\n\r\n#JuLiEventsGen tr:nth-child(even) {\r\n    background-color: #f2f2f2;\r\n}\r\n\r\n#JuLiEventsGen th {\r\n    padding-top: 12px;\r\n    padding-bottom: 12px;\r\n    text-align: left;\r\n    background-color: #4CAF50;\r\n    color: white;\r\n}\r\n</style>\r\n");

            var cssStyle = new TableCssStyle();
            cssStyle.CssClass = "table";
            //cssStyle.HeaderStyle = "font-family: Montserrat, Arial, Helvetica, sans-serif; font-weight: bold; background-color: rgb(255, 237, 0); color: rgb(229, 0, 125);";
            //cssStyle.CellStyle = "font-family: Montserrat, Arial, Helvetica, sans-serif; border: 1px solid rgb(255, 237, 0); padding: 8px;";

            using (ITableBuilder tableBuilder = new TableBuilder(stringBuilderHtmlTabelle, cssStyle)) {
                using (var headerRow = tableBuilder.AddHeaderRow()) {
                    headerRow.AddCell("Veranstaltung");
                    headerRow.AddCell("Datum");
                    headerRow.AddCell("Ort");
                }

                tableBuilder.StartTableBody();

                foreach (var curVeranstaltung in sortierteVeranstaltungen) {
                    using (var curRow = tableBuilder.AddRow()) {
                        var title = BaueVeranstaltungsString(curVeranstaltung);
                        curRow.AddCell(title);
                        curRow.AddCell(BaueDatumUhrzeitString(curVeranstaltung));
                        curRow.AddCell(BaueStadtOrtString(curVeranstaltung));
                    }
                }

                tableBuilder.EndTableBody();
            }

            var erklaerungsText = BaueErklaerungsText(geparstePages);
            stringBuilderHtmlTabelle.Append($"\r\n<p>{erklaerungsText}</p>\r\n");
            var html = stringBuilderHtmlTabelle.ToString();

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
            return
                $"Diese Übersicht fasst die Termine von {string.Join(", ", verbaende)} und {geparstePages.Last().NameDesVerbandes} zusammen";
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

        private string BaueVeranstaltungsString(VeranstaltungMitVerband veranstaltung) {
            var titleLink = FormatiereHtmlLink(ErsetzeLeerstring(veranstaltung.Title), veranstaltung.Veranstalter.FacebookDesktopUrl);
            return titleLink;
        }

        private string BaueDatumUhrzeitString(Veranstaltung veranstaltung) {
            var datumString = ErsetzeLeerstring(FormatiereDatum(veranstaltung.ZeitStart));
            var uhrzeitString = FormatiereUhrzeit(veranstaltung.ZeitStart);

            if (string.IsNullOrWhiteSpace(uhrzeitString)) return datumString;
            return $"{datumString}{Environment.NewLine}{uhrzeitString} Uhr";
        }

        private string BaueStadtOrtString(VeranstaltungMitVerband veranstaltung) {
            var str = string.Empty;
            
            if (!string.IsNullOrWhiteSpace(veranstaltung.Veranstalter.Name)) {
                str = $"{veranstaltung.Veranstalter.Name}{Environment.NewLine}{Environment.NewLine}";
            }

            if (!string.IsNullOrWhiteSpace(veranstaltung.Stadt)) {
                str = $"{str}{veranstaltung.Stadt}{Environment.NewLine}{Environment.NewLine}";
            }

            if (!string.IsNullOrWhiteSpace(veranstaltung.Ort)) {
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