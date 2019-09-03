using System.Text.RegularExpressions;

namespace FacebookEventParser.DTO
{
    public class RegExDateTimeFormat
    {
        public RegExDateTimeFormat(Regex regEx, string dateTimeFormat, bool infoUeberJahrVorhanden = true)
        {
            RegEx = regEx;
            DateTimeFormat = dateTimeFormat;
            InfoUeberJahrVorhanden = infoUeberJahrVorhanden;
        }

        public Regex RegEx { get; set; }
        public string DateTimeFormat { get; set; }
        public bool InfoUeberJahrVorhanden { get; set; }
    }
}