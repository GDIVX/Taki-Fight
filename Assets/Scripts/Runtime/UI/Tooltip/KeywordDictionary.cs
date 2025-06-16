using System.Collections.Generic;
using System.Linq;
using Runtime.CardGameplay.Card.View;

namespace Runtime.UI.Tooltip
{
    public static class KeywordDictionary
    {
        private static readonly Dictionary<string, Keyword> Keywords = new();

        public static void Register(Keyword keyword)
        {
            Keywords[keyword.Header] = keyword;
        }

        public static Keyword Get(string key)
        {
            return Keywords.GetValueOrDefault(key);
        }

        public static Keyword GetFormated(string formatedKeyword)
        {
            return Keywords.FirstOrDefault(entry => entry.Value.FormattedText.Contains(formatedKeyword)).Value;
        }

        public static bool Contain(string keywordString)
        {
            return Keywords.ContainsKey(keywordString);
        }
    }
}