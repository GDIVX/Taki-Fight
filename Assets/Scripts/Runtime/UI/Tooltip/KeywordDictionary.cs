using System.Collections.Generic;
using Runtime.CardGameplay.Card.View;

namespace Runtime.UI.Tooltip
{
    public static class KeywordDictionary
    {
        internal static readonly Dictionary<string, Keyword> Keywords = new();

        public static void Register(Keyword keyword)
        {
            Keywords[keyword.Header] = keyword;
        }

        public static Keyword Get(string key)
        {
            return Keywords.GetValueOrDefault(key);
        }
    }
}