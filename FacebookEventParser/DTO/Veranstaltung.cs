using System;

namespace FacebookEventParser.DTO
{
    public class Veranstaltung {
        public string Title { get; set; }
        public DateTime ZeitStart { get; set; }
        public string Ort { get; set; }
        public string Stadt { get; set; }

        public override string ToString()
        {
            return Title;
        }
    }

    public class Event {
        //Englische Übersetzung von Veranstaltung für den JSON-Export
        public string Title { get; set; }
        public DateTime TimeStart { get; set; }
        public string Location { get; set; }
        public string City { get; set; }

        public override string ToString()
        {
            return Title;
        }

        internal static Event CreateFromVeranstalltung(Veranstaltung veranstaltung) {
            return new Event() {
                Title = veranstaltung.Title,
                TimeStart = veranstaltung.ZeitStart,
                Location = veranstaltung.Ort,
                City = veranstaltung.Stadt
            };
        }
    }
}