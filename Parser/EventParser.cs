using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using JuLiMl.Selenium;
using Microsoft.Extensions.Logging;

namespace JuLiMl.Parser
{
    public interface IEventParser
    {
        Veranstaltung ParseVeranstaltung(string eventText);
    }

    public class EventParser : IEventParser
    {
        private readonly ILogger<EventParser> _logger;
        private readonly IRegExProvider _regExProvider;
        private readonly CultureInfo _deCulture;
        private string _eventText;
        private Veranstaltung _veranstaltung;


        public EventParser(ILogger<EventParser> logger, IRegExProvider regExProvider)
        {
            _logger = logger;
            _regExProvider = regExProvider;
            _deCulture = CultureInfo.GetCultureInfo("de-de");
        }

        public Veranstaltung ParseVeranstaltung(string eventText)
        {
            _eventText = eventText.Trim();
            _logger.LogInformation($"Starte Parsen von ###{_eventText}###");
            _veranstaltung = new Veranstaltung();

            ErsetzeMonatsAbkuerzungen();

            var datum = ParseDatum();
            var zeitStart = ParseZeitStart();

            _veranstaltung.Title = null;
            _veranstaltung.ZeitStart = datum.AddHours(zeitStart.Hour).AddMinutes(zeitStart.Minute);
            _veranstaltung.Stadt = ParseStadt();
            _veranstaltung.Ort = ParseOrt();

            _logger.LogInformation($"Parsen beendet: {_veranstaltung.ToString()}");
            return _veranstaltung;
        }

        private void ErsetzeMonatsAbkuerzungen()
        {
            _eventText = _eventText
                .Replace(" Jan. ", " Januar ")
                .Replace(" Feb. ", " Februar ")
                .Replace(" Febr. ", " Februar ")
                .Replace(" Mär. ", " März ")
                .Replace(" Mrz. ", " März ")
                .Replace(" Apr. ", " April ")
                .Replace(" Mai. ", " Mai ")
                .Replace(" Jun. ", " Juni ")
                .Replace(" Jul. ", " Juli ")
                .Replace(" Aug. ", " August ")
                .Replace(" Sep. ", " September ")
                .Replace(" Sept. ", " September ")
                .Replace(" Okt. ", " Oktober ")
                .Replace(" Nov. ", " November ")
                .Replace(" Dez. ", " Dezember ");
        }

        private DateTime ParseDatum()
        {
            var regExe = new List<RegExDateTimeFormat>()
            {
                // 01. September 2019
                new RegExDateTimeFormat(_regExProvider.GetRegex(@"(\d{2}. \w{4,9} \d{4})"), "dd. MMMM yyyy"),
                // 1. September 2019
                new RegExDateTimeFormat(_regExProvider.GetRegex(@"(\d{1}. \w{4,9} \d{4})"), "d. MMMM yyyy"),
                // 01.09.2019
                new RegExDateTimeFormat(_regExProvider.GetRegex(@"(\d{2}. \w{4,9} \d{4})"), "dd.MM.yyyy"),

                //Jetzt komme die genailen Formate ohne Jahr :/
                // 01. September
                new RegExDateTimeFormat(_regExProvider.GetRegex(@"(\d{2}. \w{4,9})"), "dd. MMMM", false),
                // 1. September
                new RegExDateTimeFormat(_regExProvider.GetRegex(@"(\d{1}. \w{4,9})"), "d. MMMM yyyy", false),
                // 01.09
                new RegExDateTimeFormat(_regExProvider.GetRegex(@"(\d{2}. \w{4,9})"), "dd.MM", false),
            };

            var firstMatch = regExe.FirstOrDefault(x => x.RegEx.IsMatch(_eventText));
            if (firstMatch == null)
            {
                _logger.LogError($"Im Event-Text ###{_eventText}###{Environment.NewLine} wurde kein Datum gefunden");
                return DateTime.MinValue;
            }

            var dateString = firstMatch.RegEx.Match(_eventText).Groups[1].Value;
            if (!DateTime.TryParseExact(dateString, firstMatch.DateTimeFormat, _deCulture, DateTimeStyles.None,
                out var outDate))
            {
                _logger.LogError($"Der RegEx entspricht nicht dem Format {firstMatch.DateTimeFormat}");
                return DateTime.MinValue;
            }

            if (!firstMatch.InfoUeberJahrVorhanden)
            {
                var curDate = DateTime.Now;
                if ((outDate.Month > curDate.Month) | (outDate.Month == curDate.Month & outDate.Day >= curDate.Day))
                {
                    return new DateTime(curDate.Year, outDate.Month, outDate.Day);
                }
                outDate = new DateTime(curDate.Year + 1, outDate.Month, outDate.Day);
            }

            return outDate;
        }

        private DateTime ParseZeitStart()
        {
            //09:00 bis 12:30
            //9:00 bis 12:30
            var startZeitMatch = _regExProvider.GetRegex(@"\b(\d{1,2}:\d{2})\b\D*").Match(_eventText);
            if (startZeitMatch.Success)
            {
                var dateStartStr = startZeitMatch.Groups[1].Value;
                DateTime startDate;
                if (DateTime.TryParseExact(dateStartStr, "HH:mm", _deCulture, DateTimeStyles.None, out startDate))
                {
                    return startDate;
                }

                if (DateTime.TryParseExact(dateStartStr, "H:mm", _deCulture, DateTimeStyles.None, out startDate))
                {
                    return startDate;
                }

                _logger.LogError(
                    $"Zeit Start konnte nicht geparsed werden aus ###{_eventText}###. RegExResult: {dateStartStr}");
                startDate = DateTime.MinValue;
                return startDate;
            }

            _logger.LogError($"Zeit Start konnte durch RegEx nicht gefunden werden: ###{_eventText}###");
            return DateTime.MinValue;
        }

        private string ParseOrt()
        {
            var lines = _eventText.Split(Environment.NewLine);
            if (lines.Length < 2) return string.Empty;
            return lines[1].Trim();
        }

        private string ParseStadt()
        {
            var lines = _eventText.Split(Environment.NewLine);
            if (lines.Length < 4) return string.Empty;
            return lines[2].Trim();
        }
    }

    public class RegExDateTimeFormat
    {
        public RegExDateTimeFormat(Regex regEx, string dateTimeFormat, bool infoUeberJahrVorhanden = true)
        {
            RegEx = regEx;
            DateTimeFormat = dateTimeFormat;
            InfoUeberJahrVorhanden = infoUeberJahrVorhanden;
        }

        public Regex RegEx { get; set; }
        public string DateTimeFormat { get; set; }
        public bool InfoUeberJahrVorhanden { get; set; }
    }
}