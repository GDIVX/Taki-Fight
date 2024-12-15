using System;
using System.Collections.Generic;
using Runtime.CardGameplay.GlyphsBoard;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace Runtime.CardGameplay.Card.View
{
    public class CardGlyphView : MonoBehaviour
    {
        [SerializeField] private List<Image> _images;
        [SerializeField, Required] private GlyphSpriteSetting _glyphSpriteSetting;

        public void Draw(List<CardGlyph> glyphs)
        {
            _images.ForEach(image => image.gameObject.SetActive(false));
            for (int i = 0; i < glyphs.Count && i < _images.Count; i++)
            {
                var glyph = glyphs[i];
                var image = _images[i];

                image.sprite = _glyphSpriteSetting.GetSprite(glyph);
                image.color = _glyphSpriteSetting.GetColor(glyph);
                image.gameObject.SetActive(true);
            }
        }
    }
}