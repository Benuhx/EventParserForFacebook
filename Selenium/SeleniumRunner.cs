using System.Collections.Generic;

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

        public SeleniumRunner(ISeleniumService seleniumService, ISeleniumInstanceService seleniumInstanceService, 
            IEventTabellenParser eventTabellenParser)
        {
            _seleniumService = seleniumService;
            _seleniumInstanceService = seleniumInstanceService;
            _eventTabellenParser = eventTabellenParser;
        }

        public void Run()
        {
            var pagesZumParsen = new List<string>()
            {
                "julisdortmund",
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
                    var curUrl = GetUrlOfPage(curPage);
                    eventTabellen = _seleniumService.IdentifiziereEventTabelle(curUrl);
                    var eventsDieserUrl = _eventTabellenParser.ParseEventTabellen(eventTabellen);
                    events.Add(new Verbandsebene(curPage, eventsDieserUrl));
                }
            }
            finally
            {
                _seleniumInstanceService.Dispose();
            }
        }

        private string GetUrlOfPage(string pageName)
        {
            return $"https://mobile.facebook.com/pg/{pageName}/events/";
        }
    }

    public class Verbandsebene
    {
        public Verbandsebene(string name, List<Veranstaltung> veranstaltungen)
        {
            Name = name;
            Veranstaltungen = veranstaltungen;
        }

        public string Name { get; set; }
        public List<Veranstaltung> Veranstaltungen { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}