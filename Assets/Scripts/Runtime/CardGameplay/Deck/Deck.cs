using System;
using System.Collections.Generic;
using System.Linq;
using Runtime.CardGameplay.Card;
using Sirenix.OdinInspector;
using UnityEngine;
using Utilities;

namespace Runtime.CardGameplay.Deck
{
    [Serializable]
    public class Deck
    {
        [ShowInInspector, ReadOnly, TableList] private Stack<CardInstance> _drawPile;
        [ShowInInspector, ReadOnly, TableList] private Stack<CardInstance> _discardPile;

        public event Action<Stack<CardInstance>> OnDrawPileUpdated;
        public event Action<Stack<CardInstance>> OnDiscardPileUpdated;

        public Deck(List<CardInstance> cards)
        {
            Setup(cards);
        }

        public void Setup(List<CardInstance> cards)
        {
            _drawPile = new Stack<CardInstance>(cards);
            _discardPile = new Stack<CardInstance>();
            Reshuffle();

            OnDrawPileUpdated?.Invoke(_drawPile);
            OnDiscardPileUpdated?.Invoke(_discardPile);
        }

        public CardInstance Draw()
        {
            if (_drawPile.Count == 0)
            {
                Reshuffle();
            }

            var card = _drawPile.Pop();
            OnDrawPileUpdated?.Invoke(_drawPile);
            return card;
        }

        public void Discard(CardInstance card)
        {
            _discardPile.Push(card);
            OnDiscardPileUpdated?.Invoke(_discardPile);
        }

        public void Reshuffle()
        {
            MergePiles();
            var cards = ShuffleCards();

            // Clear the draw pile and add the shuffled cards back
            _drawPile.Clear();
            foreach (var card in cards)
            {
                _drawPile.Push(card);
            }

            OnDrawPileUpdated?.Invoke(_drawPile);
        }

        private List<CardInstance> ShuffleCards()
        {
            var cards = new List<CardInstance>(_drawPile);
            cards.Shuffle();
            return cards;
        }

        private void MergePiles()
        {
            while (_discardPile.Count > 0)
            {
                _drawPile.Push(_discardPile.Pop());
            }
        }

        public bool IsDiscarded(CardInstance card)
        {
            return _discardPile.Contains(card);
        }

        public bool IsInDrawPile(CardInstance card)
        {
            return _drawPile.Contains(card);
        }

        public bool Exist(CardInstance card)
        {
            return IsDiscarded(card) || IsInDrawPile(card);
        }
    }
}