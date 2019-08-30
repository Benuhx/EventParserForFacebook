using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace JuLiMl.Parser
{
    public interface IRegExProvider
    {
        Regex GetRegex(string pattern);
    }
    public class RegExProvider : IRegExProvider
    {
        private readonly Dictionary<string, Regex> _regExListe;

        public RegExProvider()
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