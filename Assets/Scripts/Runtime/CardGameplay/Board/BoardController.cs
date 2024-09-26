using System;
using System.Collections;
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
        [SerializeField, Required] private RandomCardValuesPicker randomCardValuesPicker;
        [ShowInInspector, ReadOnly] private readonly Stack<CardController> _sequence = new();
        [ShowInInspector, ReadOnly] private readonly Stack<CardController> _aside = new();
        [ShowInInspector, ReadOnly] private bool _isSequenceIsIntact;

        [ShowInInspector, ReadOnly] private Suit CurrentSuit { get; set; }
        [ShowInInspector, ReadOnly] private int CurrentNumber { get; set; }

        public int SequenceCount => _sequence.Count;
        public bool IsSequenceIsIntact => _isSequenceIsIntact;


        public event Action<CardController> OnCardAdded;
        public event Action<CardController> OnCardSetAside;
        public event Action<CardController> OnCardRemoved;
        public event Action<Suit, int> OnMatchValuesChanged;


        private void OnValidate()
        {
            randomCardValuesPicker ??= FindObjectOfType<RandomCardValuesPicker>();
        }

        public bool AddToSequence(CardController cardController)
        {
            if (!IsSequenceIsIntact) return false;

            _sequence.Push(cardController);
            OnCardAdded?.Invoke(cardController);

            TestIfCardMatchCurrentValues(cardController.instance, out _isSequenceIsIntact);
            UpdateCurrentSuitAndNumber(cardController.Suit, cardController.Number);

            return true;
        }

        public void OnTurnStart()
        {
            if (!_sequence.Any())
            {
                UpdateCurrentSuitAndNumber();
            }

            //The sequance must be intact on the start of the turn
            _isSequenceIsIntact = true;
        }

        public void OnTurnEnd()
        {
            StartCoroutine(HandleCardPlaying());
        }

        private IEnumerator HandleCardPlaying()
        {
            while (_sequence.Count > 0)
            {
                var card = Remove();
                card.Play();
                float playTime = card.PlayDuration;
                yield return new WaitForSeconds(playTime);

                if (_isSequenceIsIntact)
                {
                    SetCardAside(card);
                }
                else
                {
                    HandController.Instance.Deck.Discard(card.instance);
                }
            }

            yield return new WaitForSeconds(.5f);

            //if the sequance is borken, discard the card set aside back to the deck
            if (_isSequenceIsIntact) yield break;
            {
                while (_aside.Count > 0)
                {
                    var card = _aside.Pop();
                    HandController.Instance.Deck.Discard(card.instance);
                }
            }
        }

        private void SetCardAside(CardController card)
        {
            _aside.Push(card);
            OnCardSetAside?.Invoke(card);
        }


        private void TestIfCardMatchCurrentValues(CardInstance card, out bool isSequenceIsInstance)
        {
            //The rules of the game are so:
            //1. Cards of the same suite match
            //2. Cards of equal or sequential numbers (both positive and negative) match


            switch (card.Suit)
            {
                case Suit.Black:
                    //Black always fails
                    isSequenceIsInstance = false;
                    return;
                case Suit.White:
                    isSequenceIsInstance = true;
                    return;
            }

            if (card.Suit == CurrentSuit)
            {
                isSequenceIsInstance = true;
                return;
            }

            if (card.number == CurrentNumber)
            {
                isSequenceIsInstance = true;
                return;
            }

            if (Mathf.Abs(card.number - CurrentNumber) <= 1
                || card.number == 1 && CurrentNumber == 7
                || card.number == 7 && CurrentNumber == 1)
            {
                isSequenceIsInstance = true;
                return;
            }

            isSequenceIsInstance = false;
        }

        public CardController Remove()
        {
            if (_sequence.Count == 0) return null;
            var card = _sequence.Pop();
            OnCardRemoved?.Invoke(card);
            UpdateCurrentSuitAndNumber();

            //Since we were able to place this card, we assume that the sequance is intanct once removed
            _isSequenceIsIntact = true;
            return card;
        }

        private CardController GetPrevious(CardController cardController)
        {
            if (!_sequence.Contains(cardController)) return null;

            var list = _sequence.ToList();
            var index = list.IndexOf(cardController);

            if (index == list.Count() - 1) return null;

            return list[index + 1];
        }

        private void UpdateCurrentSuitAndNumber(Suit suit, int number)
        {
            CurrentSuit = suit;
            CurrentNumber = number;
            OnMatchValuesChanged?.Invoke(CurrentSuit, CurrentNumber);
        }

        private void UpdateCurrentSuitAndNumber()
        {
            if (!_sequence.Any())
            {
                var values = randomCardValuesPicker.GetRandomValues();
                UpdateCurrentSuitAndNumber(values.Item1, values.Item2);
                return;
            }

            var nextCard = _sequence.Peek();
            if (!nextCard)
            {
                Debug.LogError($"Sequence is not empty yet peek resulted in null : {_sequence}");
                return;
            }

            UpdateCurrentSuitAndNumber(nextCard.Suit, nextCard.Number);
        }
    }
}