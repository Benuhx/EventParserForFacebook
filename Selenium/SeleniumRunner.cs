using SixLabors.ImageSharp;

namespace JuLiMl.Selenium
{
    public interface ISeleniumRunner
    {
        void Run();
    }

    public class SeleniumRunner : ISeleniumRunner
    {
        private readonly ISeleniumService _seleniumService;
        private readonly IEventBoxenService _eventBoxenService;
        private readonly ISeleniumInstanceService _seleniumInstanceService;
        private readonly IEventTabellenParser _eventTabellenParser;

        public SeleniumRunner(ISeleniumService seleniumService, IEventBoxenService eventBoxenService,
            ISeleniumInstanceService seleniumInstanceService, IEventTabellenParser eventTabellenParser)
        {
            _seleniumService = seleniumService;
            _eventBoxenService = eventBoxenService;
            _seleniumInstanceService = seleniumInstanceService;
            _eventTabellenParser = eventTabellenParser;
        }

        public void Run()
        {
            ParserResults eventTabellen = null;
            try
            {
                var url = "https://mobile.facebook.com/pg/julisdortmund/events/";
                eventTabellen = _seleniumService.IdentifiziereEventTabelle(url);
            }
            finally
            {
                _seleniumInstanceService.Dispose();
            }

            var events = _eventTabellenParser.ParseEventTabellen(eventTabellen);
            var z = 1;
        }
    }
}