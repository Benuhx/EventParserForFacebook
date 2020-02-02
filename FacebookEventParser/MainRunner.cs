using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FacebookEventParser.Config;
using FacebookEventParser.DTO;
using FacebookEventParser.OutputServices;
using FacebookEventParser.Selenium;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using TelegramApi;
using WordpressPublisher;

namespace FacebookEventParser {
    public interface IMainRunner {
        Task Run();
    }

    public class MainRunner : IMainRunner {
        private readonly ISeleniumService _seleniumService;
        private readonly ISeleniumInstanceService _seleniumInstanceService;
        private readonly IEventTabellenParser _eventTabellenParser;
        private readonly IHtmlService _htmlService;
        private readonly ILogger<MainRunner> _logger;
        private readonly IWordPressApi _wordPressApi;
        private readonly ITelegramApi _telegramApi;
        private readonly IYamlConfigService _configService;
        private readonly IFileWriter _fileWriter;

        public MainRunner(ISeleniumService seleniumService, ISeleniumInstanceService seleniumInstanceService, IEventTabellenParser eventTabellenParser, IHtmlService htmlService, ILogger<MainRunner> logger, IWordPressApi wordPressApi, ITelegramApi telegramApi, IYamlConfigService configService, IFileWriter fileWriter) {
            _seleniumService = seleniumService;
            _seleniumInstanceService = seleniumInstanceService;
            _eventTabellenParser = eventTabellenParser;
            _htmlService = htmlService;
            _logger = logger;
            _wordPressApi = wordPressApi;
            _telegramApi = telegramApi;
            _configService = configService;
            _fileWriter = fileWriter;
        }

        public async Task Run() {
            var logStr = $"Start {DateTime.Now.ToShortDateString()} at {DateTime.Now.ToShortTimeString()}";
            _logger.LogInformation(logStr);

            if (!_configService.ConfigFileExistiert()) {
                _configService.SchreibeLeeresConfigFile();
                _logger.LogWarning("Empty Config file created. Modify this file and restart app");
                return;
            }

            var config = await _configService.LeseConfigFile();
            _telegramApi.SetTelegramBotToken(config.EnableTelegramBotIntegration, config.TelegramBotToken);

            await _telegramApi.SendeNachricht(logStr, false);

            var pagesZumParsen = config.FacebookWebsites;
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

            var eventCount = events
                .GroupBy(x => x.Name, y => y.Veranstaltungen.Count)
                .OrderByDescending(x => x.First())
                .ThenBy(x => x.Key)
                .Select(x => $"{x.Key}: {x.First()}")
                .ToList();
            logStr = $"The following events were found:{Environment.NewLine}{string.Join(Environment.NewLine, eventCount)}";
            _logger.LogInformation(logStr);
            await _telegramApi.SendeNachricht(logStr, false);
            
            if (config.WriteEventsAsHtmlToFile) {
                await _fileWriter.WriteToFile(JsonConvert.SerializeObject(events, Formatting.Indented), "output.json");
            }

            if (config.WriteEventsAsHtmlToFile) {
                var htmlTabelle = _htmlService.BaueHtml(events, pagesZumParsen);
                await _fileWriter.WriteToFile(htmlTabelle, "output.html");
            }

            if (config.UploadEventsToWordpressWebsite) {
                var htmlTabelle = _htmlService.BaueHtml(events, pagesZumParsen);
                await VeroeffentlicheHtmlBeiWordPress(config, htmlTabelle);
            }
        }

        private string GetMobileUrlOfPage(string pageName) {
            return $"https://mobile.facebook.com/pg/{pageName}/events/";
        }

        private string GetDesktopUrlOfPage(string pageName) {
            return $"https://facebook.com/pg/{pageName}/events/";
        }

        private async Task VeroeffentlicheHtmlBeiWordPress(Config.Config config, string htmlTabelle) {
            string logStr;
            try {
                var cred = config.WordPressCredentials;
                await _wordPressApi.UpdatePage(config.WordpressPageId, htmlTabelle, cred);
                logStr = "WordPress Update successful";
                _logger.LogInformation(logStr);
                await _telegramApi.SendeNachricht(logStr, false);
            }
            catch (Exception e) {
                logStr = $"Error: WordPress Update NOT successful: {e}";
                _logger.LogError(logStr);
                await _telegramApi.SendeNachricht(logStr, true);
            }
        }
    }
}