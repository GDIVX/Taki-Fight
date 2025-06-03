using Runtime.UI.Tooltip;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Runtime.CardGameplay.Card.View
{
    [CreateAssetMenu(fileName = "Keyword", menuName = "Game/Keyword", order = 0)]
    public class Keyword : TooltipData
    {
        [SerializeField] [Required] private string _formatedText;

        public string FormatedText => _formatedText;

        private void OnEnable()
        {
            KeywordDictionary.Register(this);
        }
    }
}