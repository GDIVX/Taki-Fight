using System;
using System.Collections.Generic;
using System.Linq;
using Runtime.CardGameplay.Card;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Runtime.CardGameplay.GlyphsBoard
{
    public class GlyphBoardController : MonoBehaviour
    {
        [ShowInInspector, ReadOnly] public List<CardGlyph> CurrentGlyphs { get; private set; }
        [SerializeField] private int _glyphCountToActivateOnStartBattle;

        public event Action<List<CardGlyph>> OnGlyphsChanged;
        public event Action<List<CardGlyph>, List<CardGlyph>> OnGlyphsChangedCompare;


        public void UpdateGlyphs(List<CardGlyph> newGlyphs)
        {
            var oldGlyphs = CurrentGlyphs;
            CurrentGlyphs = newGlyphs;
            OnGlyphsChanged?.Invoke(CurrentGlyphs);
            OnGlyphsChangedCompare?.Invoke(oldGlyphs, CurrentGlyphs);
        }

        public static List<CardGlyph> AllGlyphs()
        {
            return Enum.GetValues(typeof(CardGlyph)).Cast<CardGlyph>().ToList();
        }

        public bool AreGlyphsMatching(List<CardGlyph> glyphs)
        {
            return glyphs.Intersect(CurrentGlyphs).Any();
        }

        public void OnBattleStart()
        {
            var glyphsToActivate = AllGlyphs().OrderBy(glyph => Random.value)
                .Take(_glyphCountToActivateOnStartBattle)
                .ToList();

            UpdateGlyphs(glyphsToActivate);
        }


        public void ActivateAllGlyphs()
        {
            UpdateGlyphs(AllGlyphs());
        }

        public void DeactivateAllGlyphs()
        {
            CurrentGlyphs.Clear();
        }
    }
}