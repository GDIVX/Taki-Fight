using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Runtime.CardGameplay.Card;
using Runtime.CardGameplay.Card.View;
using Runtime.CardGameplay.Deck;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace Runtime.CardGameplay.GlyphsBoard
{
    public class GlyphBoardView : HorizontalCardListView
    {
        [SerializeField, Required, BoxGroup("Dependencies")]
        private GlyphBoardController _glyphBoardController;

        [SerializeField, Required] private GlyphSpriteSetting _glyphSpriteSetting;

        [SerializeField] private List<GlyphToImageMapEntry> _map;
        [SerializeField] private float _fadeDuration;
        [SerializeField] private float _minAlpha;

        private void Awake()
        {
            SubscribeToBoardControllerEvents();

            MaintainConsistencyInGlyphs();
        }

        private void MaintainConsistencyInGlyphs()
        {
            _map.ForEach(entry =>
            {
                entry.Image.sprite = _glyphSpriteSetting.GetSprite(entry.Glyph);
                entry.Image.color = _glyphSpriteSetting.GetColor(entry.Glyph);
            });
        }

        private void OnDestroy()
        {
            UnsubscribeFromBoardControllerEvents();
        }

        private void SubscribeToBoardControllerEvents()
        {
            if (_glyphBoardController == null) return;
            _glyphBoardController.OnGlyphsChanged += OnGlyphsChanged;
        }


        private void UnsubscribeFromBoardControllerEvents()
        {
            if (_glyphBoardController == null) return;
            _glyphBoardController.OnGlyphsChanged -= OnGlyphsChanged;
        }


        private void OnGlyphsChanged(List<CardGlyph> glyphs)
        {
            foreach (var mapEntry in _map)
            {
                if (glyphs.Contains(mapEntry.Glyph))
                {
                    mapEntry.Image.DOFade(1, _fadeDuration);
                }
                else
                {
                    mapEntry.Image.DOFade(_minAlpha, _fadeDuration);
                }
            }
        }
    }

    [Serializable]
    public struct GlyphToImageMapEntry
    {
        public Image Image;
        public CardGlyph Glyph;
    }
}