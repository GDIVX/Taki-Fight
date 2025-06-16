using Sirenix.OdinInspector;
using UnityEngine;

namespace Runtime.UI.Tooltip
{
    [CreateAssetMenu(fileName = "Tooltip", menuName = "Game/Tooltip", order = 0)]
    public class TooltipData : ScriptableObject , IContentTooltipSource
    {
        [SerializeField] private string _header;
        [SerializeField] private string _subtitle;
        [SerializeField, TextArea] private string _content;
        [SerializeField] private Sprite _icon;
        [SerializeField, ColorPalette] private Color _backgroundColor;
        [SerializeField, Range(0, 1)] private float _alpha = 0.95f;


        public string Header => _header;
        public string Subtitle => _subtitle;

        public string Content => _content;

        public Sprite Icon => _icon;

        public Color BackgroundColor
        {
            get
            {
                var color = new Color(_backgroundColor.r, _backgroundColor.g, _backgroundColor.b, _alpha);
                return color;
            }
        }
    }
}