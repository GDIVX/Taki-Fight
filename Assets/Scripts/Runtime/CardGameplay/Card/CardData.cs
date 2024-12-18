using System.Collections.Generic;
using Runtime.CardGameplay.Card.CardBehaviour;
using Runtime.CardGameplay.Tooltip;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Runtime.CardGameplay.Card
{
    [CreateAssetMenu(fileName = "CardData", menuName = "Card/Data", order = 0)]
    public class CardData : ScriptableObject
    {
        [SerializeField, BoxGroup("UI")] private string _title;

        [SerializeField, TextArea, BoxGroup("UI")]
        private string _description;

        [SerializeField, PreviewField, BoxGroup("UI")]
        private Sprite _image;

        [SerializeField, BoxGroup("Tooltips")] private List<TooltipData> _tooltips;

        [SerializeField, BoxGroup("Stats")] private int _glyphSlots;
        [SerializeField, BoxGroup("Stats")] private CardType _cardType;

        [SerializeField, BoxGroup("Behaviour")]
        private CardAffordabilityStrategy _affordabilityStrategy;

        [SerializeField, BoxGroup("Behaviour")]
        private CardSelectStrategy _selectStrategy;

        [SerializeField, BoxGroup("Behaviour")]
        private List<PlayStrategyData> _playStrategies;


        [SerializeField, BoxGroup("Behaviour")]
        private List<CardPostPlayStrategy> _postPlayStrategies;


        public string Title => _title;
        public string Description => _description;
        public Sprite Image => _image;
        public int GlyphSlots => _glyphSlots;
        public CardSelectStrategy SelectStrategy => _selectStrategy;
        public List<PlayStrategyData> PlayStrategies => _playStrategies;

        public CardType CardType => _cardType;

        public List<TooltipData> ToolTips => _tooltips;

        public CardAffordabilityStrategy AffordabilityStrategy => _affordabilityStrategy;

        public List<CardPostPlayStrategy> PostPlayStrategies => _postPlayStrategies;
    }

    [System.Serializable]
    public struct PlayStrategyData
    {
        public CardPlayStrategy PlayStrategy;
        public int Potency;
    }
}