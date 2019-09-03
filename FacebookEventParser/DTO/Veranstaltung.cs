using System;

namespace FacebookEventParser.DTO
{
    public class Veranstaltung
    {
        public string Title { get; set; }
        public DateTime ZeitStart { get; set; }
        public string Ort { get; set; }
        public string Stadt { get; set; }

        public override string ToString()
        {
            return Title;
        }
    }
}