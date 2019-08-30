using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using JuLiMl.Selenium;

namespace JuLiMl.OutputServices
{
    public interface IHtmlTabelleService
    {
        string BaueHtmlTabelle(List<Verbandsebene> verbaendeMitEvents);
    }

    public class HtmlTabelleService : IHtmlTabelleService
    {
        public string BaueHtmlTabelle(List<Verbandsebene> verbaendeMitEvents)
        {
            var sortierteVeranstaltungen = GetVeranstaltungMitVerband(verbaendeMitEvents);
            GeneriereHtmlTabelle(sortierteVeranstaltungen);
            return null;
        }

        private void GeneriereHtmlTabelle(IEnumerable<VeranstaltungMitVerband> sortierteVeranstaltungen)
        {
            var stringBuilderHtmlTabelle = new StringBuilder();
            stringBuilderHtmlTabelle.Append("<!DOCTYPE html>\r\n<html>\r\n<body>");
            stringBuilderHtmlTabelle.Append("<head>\r\n<style>\r\n#JuLiEventsGen {\r\n  font-family: \"Merriweather\", Arial, Helvetica, sans-serif;\r\n  border-collapse: collapse;\r\n  width: 100%;\r\n}\r\n\r\n#JuLiEventsGen thead {\r\n  font-family: \"Montserrat\", Arial, Helvetica, sans-serif;\r\n  font-weight: bold;\r\n  background-color: rgb(255, 237, 0);\r\n  color: rgb(229, 0, 125);\r\n}\r\n\r\n#JuLiEventsGen td, #JuLiEventsGen th {\r\n  border: 1px solid rgb(255, 237, 0);\r\n  padding: 8px;\r\n}\r\n\r\n#JuLiEventsGen tr:nth-child(even){background-color: #f2f2f2;}\r\n\r\n#JuLiEventsGen th {\r\n  padding-top: 12px;\r\n  padding-bottom: 12px;\r\n  text-align: left;\r\n  background-color: #4CAF50;\r\n  color: white;\r\n}\r\n</style>\r\n</head>");
            stringBuilderHtmlTabelle.Append("<body>\r\n");

            
            using (var tableBuilder = new TableBuilder(stringBuilderHtmlTabelle, "JuLiEventsGen"))
            {
                using (var headerRow = tableBuilder.AddHeaderRow())
                {
                    headerRow.AddCell("Name der Veranstaltung");
                    headerRow.AddCell("Datum");
                    headerRow.AddCell("Uhrzeit");
                    headerRow.AddCell("Stadt");
                    headerRow.AddCell("Ort");
                    headerRow.AddCell("Veranstalter");
                    headerRow.AddCell("Weitere Informationen");
                }

                tableBuilder.StartTableBody();

                foreach (var curVeranstaltung in sortierteVeranstaltungen)
                {
                    using (var curRow = tableBuilder.AddRow())
                    {
                        curRow.AddCell(curVeranstaltung.Title);
                        curRow.AddCell(FormatiereDatum(curVeranstaltung.ZeitStart));
                        curRow.AddCell(FormatiereUhrzeit(curVeranstaltung.ZeitStart, curVeranstaltung.ZeitEnde));
                        curRow.AddCell(curVeranstaltung.Stadt);
                        curRow.AddCell(curVeranstaltung.Ort);
                        curRow.AddCell(curVeranstaltung.Veranstalter.Name);
                        curRow.AddCell(FormatiereHtmlLink("Facebook-Seite", curVeranstaltung.Veranstalter.FacebookDesktopUrl));
                    }
                }

                tableBuilder.EndTableBody();
            }
            
            stringBuilderHtmlTabelle.Append("</body>\r\n</html>");

            var html = stringBuilderHtmlTabelle.ToString();
            var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "JuLi-Events.html");
            if(File.Exists(path)) File.Delete(path);
            File.WriteAllText(path, html, Encoding.UTF8);
        }

        private IEnumerable<VeranstaltungMitVerband> GetVeranstaltungMitVerband(
            IEnumerable<Verbandsebene> verbaendeMitEvents)
        {
            var veranstaltungMitVerband = verbaendeMitEvents
                .Where(x => x.Veranstaltungen != null && x.Veranstaltungen.Count > 0)
                .SelectMany(x => x.Veranstaltungen,
                    (verbandsebene, veranstaltung) => new VeranstaltungMitVerband(veranstaltung, verbandsebene))
                .OrderBy(x => x.ZeitStart)
                .ToList();
            return veranstaltungMitVerband;
        }

        private string FormatiereDatum(DateTime startTime)
        {
            var deCulture = CultureInfo.GetCultureInfo("de-De");
            return startTime.ToString("dd.MM.yyyy", deCulture);
        }

        private string FormatiereUhrzeit(DateTime curVeranstaltungZeitStart, DateTime curVeranstaltungZeitEnde)
        {
            var deCulture = CultureInfo.GetCultureInfo("de-De");
            const string patternUhrzeit = "HH:mm";

            var startZeit = curVeranstaltungZeitStart.ToString(patternUhrzeit, deCulture);
            var endZeit = curVeranstaltungZeitEnde.ToString(patternUhrzeit, deCulture);

            return $"{startZeit} bis {endZeit}";
        }

        private string FormatiereHtmlLink(string linkText, string linkUrl)
        {
            return $"<a href=\"{linkUrl}\" target=\"_blank\">{linkText}</a>";
        }
    }
}