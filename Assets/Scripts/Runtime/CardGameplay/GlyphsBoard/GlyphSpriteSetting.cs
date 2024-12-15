using System;
using System.Collections.Generic;
using System.Linq;
using Runtime.CardGameplay.Card;
using UnityEngine;

namespace Runtime.CardGameplay.GlyphsBoard
{
    [CreateAssetMenu(fileName = "Glyphs Sprite Settings", menuName = "Game/Settings/Glyphs Sprites", order = 0)]
    public class GlyphSpriteSetting : ScriptableObject
    {
        [SerializeField] private List<GlyphSpriteEntry> _entries;

        public Sprite GetSprite(CardGlyph glyph)
        {
            return _entries.First(e => e.Glyph == glyph).Sprite;
        }

        public Color GetColor(CardGlyph glyph)
        {
            return _entries.First(e => e.Glyph == glyph).Color;
        }
    }

    [Serializable]
    public struct GlyphSpriteEntry
    {
        public CardGlyph Glyph;
        public Sprite Sprite;
        public Color Color;
    }
}