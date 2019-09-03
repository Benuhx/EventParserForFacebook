namespace JuLiMl.DTO
{
    public class VeranstaltungMitVerband : Veranstaltung
    {
        public VeranstaltungMitVerband(Veranstaltung veranstaltung, Verbandsebene veranstalter)
        {
            Title = veranstaltung.Title;
            ZeitStart = veranstaltung.ZeitStart;
            //ZeitEnde = veranstaltung.ZeitEnde;
            Ort = veranstaltung.Ort;
            Stadt = veranstaltung.Stadt;
            Veranstalter = veranstalter;
        }

        public VeranstaltungMitVerband()
        {
        }

        public Verbandsebene Veranstalter { get; set; }
    }
}