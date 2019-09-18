using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using FacebookEventParser.DTO;
using FacebookEventParser.Parser;
using Microsoft.Extensions.Logging;

namespace FacebookEventParser.Selenium {
    public interface IEventTabellenParser {
        List<Veranstaltung> ParseEventTabellen(ParserResults parserResults);
    }

    public class EventTabellenParser : IEventTabellenParser {
        private readonly ILogger<EventTabellenParser> _logger;
        private readonly IEventParser _eventParser;
        private readonly RegExContainer _regExContainer;
        private readonly Regex _titleRegex;

        public EventTabellenParser(ILogger<EventTabellenParser> logger, IEventParser eventParser, RegExContainer regExContainer) {
            _logger = logger;
            _eventParser = eventParser;
            _regExContainer = regExContainer;
            _titleRegex = new Regex(@"\w{3,} für (.*) \w{3,}", RegexOptions.Singleline);
        }

        public List<Veranstaltung> ParseEventTabellen(ParserResults parserResults) {
            if (parserResults.EventText.Contains("Es gibt keine bevorstehenden Veranstaltungen.")) {
                return new List<Veranstaltung>(0);
            }

            var eventTexte = SepariereVeranstalltungen(parserResults.EventText);
            var events = new List<Veranstaltung>();
            for (var i = 0; i < eventTexte.Count; i++) {
                try {
                    var eventTitle = ExtrahiereEventTitle(parserResults.LinkTexte[i]);
                    events.Add(ParseEineVeranstalltung(eventTexte[i], eventTitle));
                }
                catch (Exception e) {
                    _logger.LogError(e.ToString());
                }
            }

            return events;
        }

        private List<string> SepariereVeranstalltungen(string eventText) {
            if (string.IsNullOrEmpty(eventText)) return new List<string>(0);
            var lines = eventText.Split(Environment.NewLine);

            var naechsteZeileIstAnderesEvent = false;
            var events = new List<string>();
            var stringBuilder = new StringBuilder();
            foreach (var curLine in lines) {
                if (naechsteZeileIstAnderesEvent) {
                    events.Add(stringBuilder.ToString());
                    stringBuilder = new StringBuilder();
                    naechsteZeileIstAnderesEvent = false;
                }

                stringBuilder.Append(curLine);
                stringBuilder.Append(Environment.NewLine);
                
                if (curLine.Contains("Veranstaltungsdetails anzeigen") || curLine.Contains("Veranstaltungsdetails ansehen")) {
                    naechsteZeileIstAnderesEvent = true;
                }
            }

            events.Add(stringBuilder.ToString());

            return events;
        }

        private string ExtrahiereEventTitle(string eventTitleText) {
            return _titleRegex.Match(eventTitleText).Groups[1].Value.Trim();
        }

        private Veranstaltung ParseEineVeranstalltung(string eventText, string eventTitle) {
            var erstellteVeranstaltung = _eventParser.ParseVeranstaltung(eventText);
            erstellteVeranstaltung.Title = eventTitle;
            return erstellteVeranstaltung;
        }
    }
}