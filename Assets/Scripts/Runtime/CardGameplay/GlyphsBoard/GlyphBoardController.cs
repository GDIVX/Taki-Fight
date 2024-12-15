using System;
using System.Collections.Generic;
using System.Linq;
using Runtime.CardGameplay.Card;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Runtime.CardGameplay.GlyphsBoard
{
    public class GlyphBoardController : MonoBehaviour
    {
        [ShowInInspector, ReadOnly] public List<CardGlyph> CurrentGlyphs { get; private set; }

        public event Action<List<CardGlyph>> OnGlyphsChanged;
        public event Action<List<CardGlyph>, List<CardGlyph>> OnGlyphsChangedCompare;


        public void UpdateGlyphs(List<CardGlyph> newGlyphs)
        {
            var oldGlyphs = CurrentGlyphs;
            CurrentGlyphs = newGlyphs;
            OnGlyphsChanged?.Invoke(CurrentGlyphs);
            OnGlyphsChangedCompare?.Invoke(oldGlyphs, CurrentGlyphs);
        }


        public bool AreGlyphsMatching(List<CardGlyph> glyphs)
        {
            return glyphs.Intersect(CurrentGlyphs).Any();
        }

        private void Start()
        {
            ActivateAllGlyphs();
        }

        public void ActivateAllGlyphs()
        {
            var allGlyphs = Enum.GetValues(typeof(CardGlyph)).Cast<CardGlyph>().ToList();
            UpdateGlyphs(allGlyphs);
        }

        public void DeactivateAllGlyphs()
        {
            CurrentGlyphs.Clear();
        }



    }
}