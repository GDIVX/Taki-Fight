﻿using System.Collections.Generic;
using Runtime.CardGameplay.Card.CardBehaviour;
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

        [SerializeField, BoxGroup("Behaviour")]
        private CardSelectStrategy _selectStrategy;

        [SerializeField, BoxGroup("Behaviour")]
        private List<PlayStrategyData> _playStrategies;


        [SerializeField, BoxGroup("Behaviour")]
        private bool _destroyCardAfterUse = false;

        [SerializeField, BoxGroup("Economy")] private GemGroup _group;
        [SerializeField, BoxGroup("Economy")] private bool _extractGems = false;


        public string Title => _title;
        public string Description => _description;
        public Sprite Image => _image;
        public CardSelectStrategy SelectStrategy => _selectStrategy;
        public List<PlayStrategyData> PlayStrategies => _playStrategies;
        public bool ExtractGems => _extractGems;


        public CardType CardType => _cardType;


        public bool DestroyCardAfterUse => _destroyCardAfterUse;

        public GemGroup Group => _group;
    }

    [System.Serializable]
    public struct PlayStrategyData
    {
        public CardPlayStrategy PlayStrategy;
        public int Potency;
    }
}