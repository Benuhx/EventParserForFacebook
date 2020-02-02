using System.Collections.Generic;
using FacebookEventParser.DTO;
using WordpressPublisher;

namespace FacebookEventParser.Config
{
    public class Config
    {
        public List<FacebookPage> FacebookWebsites { get; set; }

        public bool UploadEventsToWordpressWebsite { get; set; }
        public WordPressCredentials WordPressCredentials { get; set; }
        public int WordpressPageId { get; set; }

        public bool WriteEventsAsHtmlToFile { get; set; }
        public bool WriteEventsAsJsonToFile { get; set; }

        public bool EnableTelegramBotIntegration { get; set; }
        public string TelegramBotToken { get; set; }
    }
}
