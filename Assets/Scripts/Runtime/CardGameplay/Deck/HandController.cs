using System;
using System.Collections;
using System.Collections.Generic;
using Runtime.CardGameplay.Card;
using Sirenix.OdinInspector;
using UnityEngine;
using Utilities;

namespace Runtime.CardGameplay.Deck
{
    public class HandController : MonoService<HandController>
    {
        [SerializeField] private int _cardsToDrawPerTurn;
        [SerializeField] private int _maxHandSize;
        [SerializeField, Required] private CardFactory _cardFactory;
        [SerializeField] private float _cardMovementDelay;
        [ShowInInspector, ReadOnly] private List<CardController> _cards = new();
        [ShowInInspector] [ReadOnly] public Deck Deck { get; set; }

        public int CardsToDrawPerTurn
        {
            get => _cardsToDrawPerTurn;
            set => _cardsToDrawPerTurn = value;
        }

        public int DrawPerTurn
        {
            get => _cardsToDrawPerTurn;
            set
            {
                _cardsToDrawPerTurn = value;
                OnCardDrawPerTurnUpdated?.Invoke(value);
            }
        }


        public event Action<CardController> OnCardAdded;
        public event Action<CardController> OnCardRemoved;
        public event Action<CardController> OnCardBurnt;
        public event Action<int> OnCardDrawPerTurnUpdated;

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
            if (!Deck.Draw(out var cardInstance)) return;
            InstantiateCard(cardInstance);
        }

        private void InstantiateCard(CardInstance cardInstance)
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

            InstantiateCard(cardInstance);
        }


        public void DrawHand()
        {
            if (!isActiveAndEnabled) return;
            StartCoroutine(DrawWithDelay());
        }

        private IEnumerator DrawWithDelay()
        {
            var cardsToDraw = _cardsToDrawPerTurn;
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

        public void ConsumeCard(CardController cardController)
        {
            if (Deck.IsConsumed(cardController.Instance))
            {
                Debug.LogWarning("Trying to consume a consumed card");
                return;
            }

            Deck.Consume(cardController.Instance);
            RemoveCard(cardController);
            cardController.OnDiscard();
            cardController.View.OnBurn();
            OnCardBurnt?.Invoke(cardController);
        }

        /// <summary>
        ///     The card is removed from all interaction with the game while still being tracked by the game.
        ///     Effectivly it is put in limbo.It is no longer in the game, but it is also not destroyed.
        ///     Used when you need to temporarily remove a card from the game, but you still want to keep track of it.
        /// </summary>
        /// <param name="cardController"></param>
        public void LimboCard(CardController cardController)
        {
            Deck.TryToFindAndRemoveCard(instance => cardController.Instance.Equals(instance), out _);
            RemoveCard(cardController);
            cardController.View.OnBurn();
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

        public bool HandIsEmpty()
        {
            return _cards.Count == 0;
        }
    }
}