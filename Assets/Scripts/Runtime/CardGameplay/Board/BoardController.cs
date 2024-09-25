using System;
using System.Collections.Generic;
using System.Linq;
using Runtime.CardGameplay.Card;
using Runtime.CardGameplay.Deck;
using Sirenix.OdinInspector;
using Unity.VisualScripting;
using UnityEngine;

namespace Runtime.CardGameplay.Board
{
    public class BoardController : Utilities.Singleton<BoardController>
    {
        [ShowInInspector, ReadOnly] private readonly Stack<CardController> _sequence = new();

        public int SequenceCount => _sequence.Count;

        public event Action<CardController> OnCardAdded;
        public event Action<string> OnCardAddedExplainReason;
        public event Action<CardController> OnCardRemoved;

        public bool AddToSequence(CardController cardController)
        {
            if (!IsSequenceIsIntact()) return false;

            _sequence.Push(cardController);
            OnCardAdded?.Invoke(cardController);
            return true;
        }

        public bool IsSequenceIsIntact()
        {
            // We can only do compression between two cards. So treat any less as a valid sequence
            if (_sequence.Count is 0 or 1) return true;

            //The rules of the game are so:
            //1. Cards of the same suite match
            //2. Cards of equal or sequential numbers (both positive and negative) match

            var lastCard = _sequence.Peek();

            if (lastCard == null)
            {
                Debug.LogError("Last card in the sequence is null");
                return false;
            }

            var previousCard = GetPrevious(lastCard);
            if (previousCard == null)
            {
                Debug.LogError("Previous card in the sequence is null");
                return false;
            }

            if (lastCard.Suit == Suit.Black)
            {
                //Black always fails
                OnCardAddedExplainReason?.Invoke("Black Suit");
                return false;
            }

            if (lastCard.Suit == Suit.White)
            {
                OnCardAddedExplainReason?.Invoke("Joker");
                return true;
            }

            if (lastCard.Suit == previousCard.Suit)
            {
                OnCardAddedExplainReason?.Invoke($"Same Suite : {lastCard.Suit}");
                return true;
            }

            if (Mathf.Abs(lastCard.Number - previousCard.Number) <= 1)
            {
                OnCardAddedExplainReason?.Invoke($"Sequential Numbers: {lastCard.Number} AND {previousCard.Number}");
                return true;
            }

            OnCardAddedExplainReason?.Invoke("Cards Don't Match");
            return false;
        }

        public CardController Remove()
        {
            if (_sequence.Count == 0) return null;
            var card = _sequence.Pop();
            OnCardRemoved?.Invoke(card);
            return card;
        }

        public void DiscardAll()
        {
            while (_sequence.Count > 0)
            {
                var card = Remove();
                HandController.Instance.Deck.Discard(card.Instance);
            }
        }

        public CardController GetPrevious(CardController cardController)
        {
            if (!_sequence.Contains(cardController)) return null;

            var list = _sequence.ToList();
            var index = list.IndexOf(cardController);

            if (index == list.Count() - 1) return null;

            return list[index + 1];
        }
    }
}