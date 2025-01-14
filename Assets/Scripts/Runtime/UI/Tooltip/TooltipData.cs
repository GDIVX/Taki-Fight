using Sirenix.OdinInspector;
using UnityEngine;

namespace Runtime.UI.Tooltip
{
    [CreateAssetMenu(fileName = "Tooltip", menuName = "Game/Tooltip", order = 0)]
    public class TooltipData : ScriptableObject
    {
        [SerializeField] private string _header;
        [SerializeField] private string _secondHeader;
        [SerializeField , TextArea] private string _description;
        [SerializeField] private Sprite _icon;
        [SerializeField, ColorPalette] private Color _backgroundColor;


        public string Header => _header;
        public string SecondHeader => _secondHeader;

        public string Description => _description;

        public Sprite Icon => _icon;

        public Color BackgroundColor => _backgroundColor;
    }
}