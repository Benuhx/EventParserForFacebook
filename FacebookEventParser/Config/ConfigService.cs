using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using FacebookEventParser.DTO;
using Microsoft.Extensions.Logging;
using WordpressPublisher;
using YamlDotNet.Serialization;

namespace FacebookEventParser.Config {
    public interface IYamlConfigService {
        bool ConfigFileExistiert();
        void SchreibeLeeresConfigFile();
        Task<Config> LeseConfigFile();
    }

    public class YamlConfigService : IYamlConfigService {
        private readonly string _configFilePath;
        private readonly ILogger<YamlConfigService> _logger;

        public YamlConfigService(ILogger<YamlConfigService> logger) {
            _logger = logger;
            var currentDir = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            _configFilePath = Path.Combine(currentDir, "config.yaml");
        }

        public bool ConfigFileExistiert() {
            return File.Exists(_configFilePath);
        }

        public async Task<Config> LeseConfigFile() {
            var deserializer = new Deserializer();

            if (!ConfigFileExistiert()) _logger.LogError($"Es existiert kein Config-File unter '{_configFilePath}' :(");

            var fileContent = await File.ReadAllTextAsync(_configFilePath);
            var config = deserializer.Deserialize<Config>(fileContent);

            return config;
        }


        public void SchreibeLeeresConfigFile() {
            var emptyConfig = new Config() {
                FacebookWebsites = new List<FacebookPage>() {new FacebookPage("JuLis Bundesverband", "jungeliberale")},
                WordPressCredentials = new WordPressCredentials("InsertWordpressUsernameHere", "InsertWordpressPasswordHere", "InsertWordpressUrlHere"),
                EnableTelegramBotIntegration = false
            };

            var serializer = new Serializer();
            var yaml = serializer.Serialize(emptyConfig);

            _logger.LogInformation($"Schreibe default config nach {_configFilePath}");

            if (File.Exists(_configFilePath)) File.Delete(_configFilePath);
            File.WriteAllText(_configFilePath, yaml);
        }
    }
}