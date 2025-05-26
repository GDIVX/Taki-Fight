using System;
using System.Collections.Generic;
using Runtime.CardGameplay.Card.CardBehaviour;
using Runtime.CardGameplay.Card.CardBehaviour.Feedback;
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

        [SerializeField, BoxGroup("Stats")] private CardType _cardType;
        [SerializeField, BoxGroup("Stats")] private Rarity _rarity;


        [SerializeField, BoxGroup("Behaviour")]
        private List<PlayStrategyData> _playStrategies;


        [SerializeField, BoxGroup("Behaviour")]
        private bool _destroyCardAfterUse = false;

        [SerializeField, BoxGroup("Economy")] private int _cost;
        [SerializeField] private FeedbackStrategy _feedbackStrategy;


        public string Title
        {
            get => _title;
            set => _title = value;
        }

        public string Description
        {
            get => _description;
            set => _description = value;
        }

        public Sprite Image
        {
            get => _image;
            set => _image = value;
        }

        public List<PlayStrategyData> PlayStrategies
        {
            get => _playStrategies;
            set => _playStrategies = value;
        }


        public CardType CardType => _cardType;
        public Rarity Rarity => _rarity;

        public bool DestroyCardAfterUse => _destroyCardAfterUse;

        public int Cost => _cost;

        public FeedbackStrategy FeedbackStrategy => _feedbackStrategy;
    }

    [Serializable]
    public struct PlayStrategyData
    {
        public CardPlayStrategy PlayStrategy;
        public int Potency;
    }

    public enum Rarity
    {
        None,
        Common,
        Uncommon,
        Rare,
        Legendery
    }
}