using System;
using System.Collections;
using System.Collections.Generic;
using Runtime.CardGameplay.Card;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Runtime.CardGameplay.Deck
{
    public class HandController : MonoBehaviour
    {
        [SerializeField] private int _targetHandSize;
        [SerializeField] private int _maxHandSize;
        [SerializeField, Required] private CardFactory _cardFactory;
        [SerializeField] private float _cardMovementDelay;
        public Deck Deck { get; set; }
        [ShowInInspector, ReadOnly] private List<CardController> _cards = new();

        public event Action<CardController> OnCardAdded;
        public event Action<CardController> OnCardRemoved;
        public event Action<CardController> OnCardBurnt;
        public event Action<int> OnCardDrawPerTurnUpdated;

        public int DrawPerTurn
        {
            get => _targetHandSize;
            set
            {
                _targetHandSize = value;
                OnCardDrawPerTurnUpdated?.Invoke(value);
            }
        }

        /// <summary>
        /// Remove a card from hand
        /// </summary>
        /// <param name="cardController"></param>
        private void RemoveCard(CardController cardController)
        {
            if (!_cards.Contains(cardController)) return;
            _cards.Remove(cardController);
            OnCardRemoved?.Invoke(cardController);
        }

        /// <summary>
        /// Add a card to the hand.
        /// </summary>
        /// <param name="cardController"></param>
        private void AddCard(CardController cardController)
        {
            if (cardController == null)
            {
                Debug.LogWarning("Trying to add null card to hand");
                return;
            }

            _cards.Add(cardController);
            OnCardAdded?.Invoke(cardController);
        }

        /// <summary>
        /// Draw a card from the deck, create for it a game object and add it to the hand
        /// </summary>
        [Button]
        public void DrawCard()
        {
            if (_cards.Count >= _maxHandSize) return;
            if (!Deck.Draw(out CardInstance cardInstance)) return;
            DrawCard(cardInstance);
        }

        private void DrawCard(CardInstance cardInstance)
        {
            var controller = _cardFactory.Create(cardInstance);
            controller.OnDraw();
            controller.View.OnDraw();
            AddCard(controller);
        }

        [Button]
        public void FindAndDrawCard(CardData cardData)
        {
            if (_cards.Count >= _maxHandSize) return;
            if (!Deck.TryToFindAndRemoveCard(cardData, out CardInstance cardInstance)) return;

            DrawCard(cardInstance);
        }


        public void DrawHand()
        {
            if (isActiveAndEnabled)
            {
                StartCoroutine(DrawWithDelay());
            }
        }

        private IEnumerator DrawWithDelay()
        {
            var cardsToDraw = _targetHandSize - _cards.Count;
            if (cardsToDraw <= 0)
            {
                yield break;
            }

            for (int i = 0; i < cardsToDraw; i++)
            {
                yield return new WaitForSeconds(_cardMovementDelay);
                DrawCard();
            }
        }

        /// <summary>
        /// Discard a card to the deck's discard pile, and remove it from hand
        /// </summary>
        /// <param name="cardController"></param>
        [Button]
        public void DiscardCard(CardController cardController)
        {
            //If the card is already discard, return to avoid side effects
            if (Deck.IsDiscarded(cardController.Instance)) return;

            Deck.Discard(cardController.Instance);
            RemoveCard(cardController);
            cardController.OnDiscard();
            cardController.View.OnDiscard();
        }

        public void BurnCard(CardController cardController)
        {
            if (Deck.IsBurnt(cardController.Instance)) return;

            Deck.Burn(cardController.Instance);
            RemoveCard(cardController);
            cardController.OnDiscard();
            cardController.View.OnBurn();
            OnCardBurnt?.Invoke(cardController);
        }

        public bool Has(CardController cardController)
        {
            return _cards.Contains(cardController);
        }

        public void DiscardHand()
        {
            StartCoroutine(DiscardHandWithDelay());
        }

        private IEnumerator DiscardHandWithDelay()
        {
            // Create a copy of the list to avoid modifying it while iterating
            List<CardController> cardsToDiscard = new List<CardController>(_cards);

            foreach (var card in cardsToDiscard)
            {
                yield return new WaitForSeconds(_cardMovementDelay);
                DiscardCard(card);
            }
        }
    }
}