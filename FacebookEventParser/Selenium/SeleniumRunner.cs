using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using FacebookEventParser.DTO;
using FacebookEventParser.OutputServices;
using Microsoft.Extensions.Logging;
using TelegramApi;
using WordpressPublisher;

namespace FacebookEventParser.Selenium {
    public interface ISeleniumRunner {
        Task Run();
    }

    public class SeleniumRunner : ISeleniumRunner {
        private readonly ISeleniumService _seleniumService;
        private readonly ISeleniumInstanceService _seleniumInstanceService;
        private readonly IEventTabellenParser _eventTabellenParser;
        private readonly IHtmlService _htmlService;
        private readonly ILogger<SeleniumRunner> _logger;
        private readonly IWordPressApi _wordPressApi;
        private readonly ITelegramApi _telegramApi;

        public SeleniumRunner(ISeleniumService seleniumService, ISeleniumInstanceService seleniumInstanceService, IEventTabellenParser eventTabellenParser, IHtmlService htmlService, ILogger<SeleniumRunner> logger, IWordPressApi wordPressApi, ITelegramApi telegramApi) {
            _seleniumService = seleniumService;
            _seleniumInstanceService = seleniumInstanceService;
            _eventTabellenParser = eventTabellenParser;
            _htmlService = htmlService;
            _logger = logger;
            _wordPressApi = wordPressApi;
            _telegramApi = telegramApi;
        }

        public async Task Run() {
            _logger.LogInformation($"Start am {DateTime.Now.ToShortDateString()} um {DateTime.Now.ToShortTimeString()} Uhr");

            var pagesZumParsen = new List<FacebookPage> {
                new FacebookPage("JuLis Bundesverband", "jungeliberale"),
                new FacebookPage("JuLis NRW", "julisnrw"),
                new FacebookPage("JuLis Ruhrgebiet", "julisruhrgebiet"),
                new FacebookPage("JuLis Dortmund", "julisdortmund"),
                new FacebookPage("JuLis Bochum", "julisbochum"),
                new FacebookPage("JuLis Essen", "julisessen"),
                new FacebookPage("JuLis Herne", "Julis-Herne-517505588327635"),
                new FacebookPage("JuLis Mülheim an der Ruhr", "julismh"),
                new FacebookPage("JuLis Gelsenkirchen", "JuLis-Gelsenkirchen-314536558629253"),
                new FacebookPage("JuLis Bottrop", "julis.bottrop"),
                new FacebookPage("JuLis Oberhausen", "oberhausen.julis"),
                new FacebookPage("JuLis Recklinghausen", "julis.kv.re")
            };

            var events = new List<Verbandsebene>();
            try {
                foreach (var curPage in pagesZumParsen) {
                    var curUrl = GetMobileUrlOfPage(curPage.NameDerFacebookPage);
                    var eventTabellen = _seleniumService.IdentifiziereEventTabelle(curUrl);
                    var eventsDieserUrl = _eventTabellenParser.ParseEventTabellen(eventTabellen);
                    events.Add(new Verbandsebene(curPage.NameDesVerbandes, GetDesktopUrlOfPage(curPage.NameDerFacebookPage), eventsDieserUrl));
                }
            }
            finally {
                _seleniumInstanceService.Dispose();
            }

            await _telegramApi.SendeNachricht($"Ich habe {events.Count} Events gefunden");

            var htmlTabelle = _htmlService.BaueHtml(events, pagesZumParsen);
            var cred = new WordPressCredentials("***REMOVED***", "***REMOVED***", "***REMOVED***");
            await _wordPressApi.UpdatePage(468, htmlTabelle, cred);

            await _telegramApi.SendeNachricht($"Und die Homepage ist auch fertig :)");
        }

        private string GetMobileUrlOfPage(string pageName) {
            return $"https://mobile.facebook.com/pg/{pageName}/events/";
        }

        private string GetDesktopUrlOfPage(string pageName) {
            return $"https://facebook.com/pg/{pageName}/events/";
        }
    }
}