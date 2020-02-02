using System.Collections.Generic;
using System.Linq;

namespace FacebookEventParser.DTO
{
    public class Verbandsebene
    {
        public Verbandsebene(string name, string facebookDesktopUrl, List<Veranstaltung> veranstaltungen)
        {
            Name = name;
            FacebookDesktopUrl = facebookDesktopUrl;
            Veranstaltungen = veranstaltungen;
        }

        public string Name { get; set; }
        public string FacebookDesktopUrl { get; set; }
        public List<Veranstaltung> Veranstaltungen { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }

    public class Association
    {
        public Association(string name, string facebookDesktopUrl, List<Event> events)
        {
            AssociationName = name;
            FacebookDesktopUrl = facebookDesktopUrl;
            Events = events;
        }

        public string AssociationName { get; set; }
        public string FacebookDesktopUrl { get; set; }
        public List<Event> Events { get; set; }

        public override string ToString()
        {
            return AssociationName;
        }

        internal static Association CreateFromVerbandsebene(Verbandsebene verbandsebene) {
            var events = verbandsebene.Veranstaltungen
                .Select(Event.CreateFromVeranstalltung)
                .ToList();
            return new Association(verbandsebene.Name, verbandsebene.FacebookDesktopUrl, events);
        }
    }
}