using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using JuLiMl.DTO;
using JuLiMl.Parser;
using Microsoft.Extensions.Logging;

namespace JuLiMl.Selenium
{
    public interface IEventTabellenParser
    {
        List<Veranstaltung> ParseEventTabellen(ParserResults parserResults);
    }

    public class EventTabellenParser : IEventTabellenParser
    {
        private readonly ILogger<EventTabellenParser> _logger;
        private readonly IEventParser _eventParser;
        private readonly Regex _datumZeitStringRegex;
        private readonly Regex _titleRegex;

        public EventTabellenParser(ILogger<EventTabellenParser> logger, IEventParser eventParser)
        {
            _logger = logger;
            _eventParser = eventParser;
            _datumZeitStringRegex = new Regex("(.*) von (.*) bis (.*) UTC .*", RegexOptions.IgnorePatternWhitespace);
            _titleRegex = new Regex("Veranstaltungsdetails für (.*) anzeigen", RegexOptions.Singleline);
        }

        public List<Veranstaltung> ParseEventTabellen(ParserResults parserResults)
        {
            if (parserResults.EventText.Contains("Es gibt keine bevorstehenden Veranstaltungen."))
                return new List<Veranstaltung>(0);

            var eventTexte = SepariereVeranstalltungen(parserResults.EventText);
            var events = new List<Veranstaltung>();
            for (var i = 0; i < eventTexte.Count; i++)
            {
                try
                {
                    var eventTitle = ExtrahiereEventTitle(parserResults.LinkTexte[i]);
                    events.Add(ParseEineVeranstalltung(eventTexte[i], eventTitle));
                }
                catch (Exception e)
                {
                    _logger.LogError(e.ToString());
                }
            }

            return events;
        }

        private List<string> SepariereVeranstalltungen(string eventText)
        {
            if (string.IsNullOrEmpty(eventText)) return new List<string>(0);
            var lines = eventText.Split(Environment.NewLine);

            var naechsteZeileIstAnderesEvent = false;
            var events = new List<string>();
            var stringBuilder = new StringBuilder();
            foreach (var curLine in lines)
            {
                if (naechsteZeileIstAnderesEvent)
                {
                    events.Add(stringBuilder.ToString());
                    stringBuilder = new StringBuilder();
                    naechsteZeileIstAnderesEvent = false;
                }

                stringBuilder.Append(curLine);
                stringBuilder.Append(Environment.NewLine);
                if (curLine.Contains("Veranstaltungsdetails anzeigen")) naechsteZeileIstAnderesEvent = true;
            }

            events.Add(stringBuilder.ToString());

            return events;
        }

        private string ExtrahiereEventTitle(string eventTitleText)
        {
            return _titleRegex.Match(eventTitleText).Groups[1].Value.Trim();
        }

        private Veranstaltung ParseEineVeranstalltung(string eventText, string eventTitle)
        {
            var erstellteVeranstaltung = _eventParser.ParseVeranstaltung(eventText);
            erstellteVeranstaltung.Title = eventTitle;
            return erstellteVeranstaltung;

            var veranstaltung = new Veranstaltung();
            var lines = eventText.Trim().Split(Environment.NewLine).ToList();
            if (lines.Count == 3)
            {
                _logger.LogWarning("Im EventText fehlt die Stadt (?)");
                lines.Insert(2, string.Empty);
            }

            if (lines.Count != 4)
                throw new InvalidDataException(
                    $"Event-Text nicht gültig, da {lines.Count} statt 4 Zeilen:{Environment.NewLine}{eventText}");

            var regexResult = _datumZeitStringRegex.Match(lines[0]);
            var datumString = regexResult.Groups[1].Value.Trim();
            var zeitStartString = regexResult.Groups[2].Value.Trim();
            var zeitEndeString = regexResult.Groups[3].Value.Trim();

            veranstaltung.Title = eventTitle;
            veranstaltung.Ort = lines[1];
            veranstaltung.Stadt = lines[2];

            var deCulture = CultureInfo.GetCultureInfo("de-de");
            var datum = DateTime.ParseExact(datumString, "dddd, d. MMMM yyyy", deCulture);
            var zeitStart = DateTime.ParseExact(zeitStartString, "HH:mm", deCulture);
            var zeitEnde = DateTime.ParseExact(zeitEndeString, "HH:mm", deCulture);

            veranstaltung.ZeitStart = datum.AddHours(zeitStart.Hour).AddMinutes(zeitStart.Minute);
            //veranstaltung.ZeitEnde = datum.AddHours(zeitEnde.Hour).AddMinutes(zeitEnde.Minute);

            return veranstaltung;
        }
    }
}