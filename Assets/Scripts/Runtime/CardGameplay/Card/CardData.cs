﻿using Runtime.CardGameplay.Card.CardBehaviour;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Runtime.CardGameplay.Card
{
    [CreateAssetMenu(fileName = "CardData", menuName = "Card/Data", order = 0)]
    public class CardData : ScriptableObject
    {
        [SerializeField] private string title;
        [SerializeField, TextArea] private string description;
        [SerializeField, PreviewField] private Sprite image;
        [SerializeField] private Suit suit;
        [SerializeField] private CardSelectStrategy selectStrategy;
        [SerializeField] private CardPlayStrategy playStrategy;
        [SerializeField] private int energyCost;

        public string Title => title;
        public string Description => description;
        public Sprite Image => image;
        public Suit Suit => suit;
        public CardSelectStrategy SelectStrategy => selectStrategy;
        public CardPlayStrategy PlayStrategy => playStrategy;
        public int EnergyCost => energyCost;
    }
}