using System.Collections.Generic;

namespace JuLiMl.DTO
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
}