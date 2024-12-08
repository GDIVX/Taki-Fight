using Runtime.CardGameplay.Card.CardBehaviour;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Runtime.CardGameplay.Card
{
    [CreateAssetMenu(fileName = "CardData", menuName = "Card/Data", order = 0)]
    public class CardData : ScriptableObject
    {
        [SerializeField] private string _title;
        [SerializeField, TextArea] private string _description;
        [SerializeField, PreviewField] private Sprite _image;
        [SerializeField] private Suit _suit;
        [SerializeField] private CardSelectStrategy _selectStrategy;
        [SerializeField] private CardPlayStrategy _playStrategy;
        [SerializeField] private int _energyCost;
        [SerializeField] private int _potency;
        [SerializeField] private CardType _cardType;

        public int Potency => _potency;
        public string Title => _title;
        public string Description => _description;
        public Sprite Image => _image;
        public Suit Suit => _suit;
        public CardSelectStrategy SelectStrategy => _selectStrategy;
        public CardPlayStrategy PlayStrategy => _playStrategy;
        public int EnergyCost => _energyCost;

        public CardType CardType => _cardType;
    }
}