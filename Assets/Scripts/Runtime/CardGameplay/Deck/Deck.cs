using System;
using System.Collections.Generic;
using Runtime.CardGameplay.Card;
using Sirenix.OdinInspector;
using UnityEngine;
using Utilities;

namespace Runtime.CardGameplay.Deck
{
    [Serializable]
    public class Deck
    {
        [ShowInInspector, ReadOnly, TableList] public Stack<CardInstance> DrawPile { get; private set; }
        [ShowInInspector, ReadOnly, TableList] public Stack<CardInstance> DiscardPile { get; private set; }

        public event Action<Stack<CardInstance>> OnDrawPileUpdated;
        public event Action<Stack<CardInstance>> OnDiscardPileUpdated;

        public Deck(List<CardInstance> cards)
        {
            Setup(cards);
        }

        public void Setup(List<CardInstance> cards)
        {
            DrawPile = new Stack<CardInstance>(cards);
            DiscardPile = new Stack<CardInstance>();
            Reshuffle();

            OnDrawPileUpdated?.Invoke(DrawPile);
            OnDiscardPileUpdated?.Invoke(DiscardPile);
        }

        public CardInstance Draw()
        {
            if (DrawPile.Count == 0)
            {
                Reshuffle();
            }

            var card = DrawPile.Pop();
            OnDrawPileUpdated?.Invoke(DrawPile);
            return card;
        }

        public void Discard(CardInstance card)
        {
            DiscardPile.Push(card);
            OnDiscardPileUpdated?.Invoke(DiscardPile);
        }

        public void Reshuffle()
        {
            MergePiles();
            var cards = ShuffleCards();

            // Clear the draw pile and add the shuffled cards back
            DrawPile.Clear();
            foreach (var card in cards)
            {
                DrawPile.Push(card);
            }

            OnDrawPileUpdated?.Invoke(DrawPile);
        }

        private List<CardInstance> ShuffleCards()
        {
            var cards = new List<CardInstance>(DrawPile);
            cards.Shuffle();
            return cards;
        }

        private void MergePiles()
        {
            while (DiscardPile.Count > 0)
            {
                DrawPile.Push(DiscardPile.Pop());
            }
        }
    }
}