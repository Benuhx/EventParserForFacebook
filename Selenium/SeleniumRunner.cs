using System;
using System.Collections.Generic;
using JuLiMl.DTO;
using JuLiMl.OutputServices;
using Microsoft.Extensions.Logging;

namespace JuLiMl.Selenium
{
    public interface ISeleniumRunner
    {
        void Run();
    }

    public class SeleniumRunner : ISeleniumRunner
    {
        private readonly ISeleniumService _seleniumService;
        private readonly ISeleniumInstanceService _seleniumInstanceService;
        private readonly IEventTabellenParser _eventTabellenParser;
        private readonly IHtmlTabelleService _htmlTabelleService;
        private readonly ILogger<SeleniumRunner> _logger;

        public SeleniumRunner(ISeleniumService seleniumService, ISeleniumInstanceService seleniumInstanceService, 
            IEventTabellenParser eventTabellenParser, IHtmlTabelleService htmlTabelleService, ILogger<SeleniumRunner> logger)
        {
            _seleniumService = seleniumService;
            _seleniumInstanceService = seleniumInstanceService;
            _eventTabellenParser = eventTabellenParser;
            _htmlTabelleService = htmlTabelleService;
            _logger = logger;
        }

        public void Run()
        {
            _logger.LogInformation($"Start am {DateTime.Now.ToShortDateString()} um {DateTime.Now.ToShortTimeString()} Uhr");
            var pagesZumParsen = new List<string>()
            {
                "julisdortmund",
                "julisnrw",
                "jungeliberale",
                "julisbochum",
                "julisessen",
                "Julis-Herne-517505588327635",
                "julismh",
                "JuLis-Gelsenkirchen-314536558629253",
                "julis.bottrop",
                "oberhausen.julis",
                "julis.kv.re",
                "julisruhrgebiet"
            };

            ParserResults eventTabellen = null;
            var events = new List<Verbandsebene>();
            try
            {
                foreach (var curPage in pagesZumParsen)
                {
                    var curUrl = GetMobileUrlOfPage(curPage);
                    eventTabellen = _seleniumService.IdentifiziereEventTabelle(curUrl);
                    var eventsDieserUrl = _eventTabellenParser.ParseEventTabellen(eventTabellen);
                    events.Add(new Verbandsebene(curPage, GetDesktopUrlOfPage(curPage), eventsDieserUrl));
                }
            }
            finally
            {
                _seleniumInstanceService.Dispose();
            }

            _htmlTabelleService.BaueHtmlTabelle(events);
        }

        private string GetMobileUrlOfPage(string pageName)
        {
            //return $"https://mobile.facebook.com/{pageName}?v=events&is_past=1";
            return $"https://mobile.facebook.com/pg/{pageName}/events/";
        }

        private string GetDesktopUrlOfPage(string pageName)
        {
            return $"https://facebook.com/pg/{pageName}/events/";
        }
    }
}