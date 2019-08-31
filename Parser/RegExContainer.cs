using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace JuLiMl.Parser
{
    public interface IRegExContainer
    {
        Regex GetRegex(string pattern);
    }
    public class RegExContainer : IRegExContainer
    {
        private readonly Dictionary<string, Regex> _regExListe;

        public RegExContainer()
        {
            _regExListe = new Dictionary<string, Regex>();
        }

        public Regex GetRegex(string pattern)
        {
            if (_regExListe.TryGetValue(pattern, out var regEx)) return regEx;

            regEx = new Regex(pattern, RegexOptions.IgnoreCase);
            _regExListe.Add(pattern, regEx);
            return regEx;
        }
    }
}