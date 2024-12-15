using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Runtime.CardGameplay.Tooltip
{
    [CreateAssetMenu(fileName = "Tooltip", menuName = "Card/Tooltip", order = 0)]
    public class CardTooltipData : ScriptableObject
    {
        [SerializeField] private Sprite _boundingBoxTexture;
        [SerializeField] private Color _color;
        [SerializeField] private string _header;
        [SerializeField, TextArea] private string _content;
        [SerializeField, Optional] private List<CardTooltipData> _attachedTooltips;

        public Sprite BoundingBoxTexture => _boundingBoxTexture;
        public Color Color => _color;
        public string HeaderText => _header;
        public string ContentText => _content;
        public List<CardTooltipData> AttachedTooltips => _attachedTooltips;
    }
}