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

        [SerializeField, PreviewField, BoxGroup("Tooltips")]
        private List<TooltipData> _tooltips;

        [SerializeField, BoxGroup("Stats")] private int _glyphSlots;
        [SerializeField, BoxGroup("Stats")] private int _potency;
        [SerializeField, BoxGroup("Stats")] private CardType _cardType;

        [SerializeField, BoxGroup("Behaviour")]
        private CardSelectStrategy _selectStrategy;

        [SerializeField, BoxGroup("Behaviour")]
        private CardPlayStrategy _playStrategy;

        [SerializeField, BoxGroup("Behaviour")]
        private CardAffordabilityStrategy _affordabilityStrategy;

        [SerializeField, BoxGroup("Behaviour")]
        private CardPostPlayStrategy _postPlayStrategy;

        [SerializeField, BoxGroup("Behaviour")]
        private CardOnRankChangedEventHandler _rankChangedEventHandler;

        [SerializeField, BoxGroup("Behaviour")]
        private CardOnSuitChangeEventHandler _suitChangedEventHandler;

        public int Potency => _potency;
        public string Title => _title;
        public string Description => _description;
        public Sprite Image => _image;
        public int GlyphSlots => _glyphSlots;
        public CardSelectStrategy SelectStrategy => _selectStrategy;
        public CardPlayStrategy PlayStrategy => _playStrategy;

        public CardType CardType => _cardType;

        public List<TooltipData> ToolTips;

        public CardAffordabilityStrategy AffordabilityStrategy => _affordabilityStrategy;

        public CardPostPlayStrategy PostPlayStrategy => _postPlayStrategy;
    }
}