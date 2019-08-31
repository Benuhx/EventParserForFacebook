namespace JuLiMl.DTO
{
    internal class FacebookPage
    {
        public FacebookPage(string nameDesVerbandes, string nameDerFacebookPage)
        {
            NameDesVerbandes = nameDesVerbandes;
            NameDerFacebookPage = nameDerFacebookPage;
        }

        public FacebookPage()
        {
        }

        public string NameDesVerbandes { get; set; }
        public string NameDerFacebookPage { get; set; }
    }
}