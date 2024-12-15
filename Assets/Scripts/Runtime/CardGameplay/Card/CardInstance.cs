using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;

namespace Runtime.CardGameplay.Card
{
    [Serializable]
    public class CardInstance
    {
        public CardData Data;
        [ShowInInspector] public List<CardGlyph> Glyphs { get; set; }

        public CardController Controller { get; set; }

        public CardInstance(CardData data, List<CardGlyph> glyphs)
        {
            Data = data;
            Glyphs = glyphs;
            Controller = null;
        }

        public CardInstance(CardData data)
        {
            Data = data;
            Controller = null;

            Glyphs = CreateGlyphs(data.GlyphSlots);
        }

        private List<CardGlyph> CreateGlyphs(int glyphSlots)
        {
            var options = Enum.GetValues(typeof(CardGlyph)).Cast<CardGlyph>().ToList();
            var result = new List<CardGlyph>();

            for (int i = 0; i < glyphSlots && options.Count > 0; i++)
            {
                int index = UnityEngine.Random.Range(0, options.Count);
                result.Add(options[index]);
                options.RemoveAt(index);
            }

            return result;
        }
    }
}