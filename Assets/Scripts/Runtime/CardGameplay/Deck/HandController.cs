using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Runtime.CardGameplay.Card;
using Sirenix.OdinInspector;
using UnityEngine;
using Utilities;

namespace Runtime.CardGameplay.Deck
{
    public class HandController : Singleton<HandController>
    {
        [SerializeField] private int cardToDrawPerTurn;
        [SerializeField] private int maxHandSize;
        public Deck Deck { get; set; }
        [ShowInInspector, ReadOnly] private List<CardController> _cards = new();

        public event Action<CardController> OnCardAdded;
        public event Action<CardController> OnCardRemoved;

        public int CardToDrawPerTurn
        {
            get => cardToDrawPerTurn;
            set => cardToDrawPerTurn = value;
        }

        /// <summary>
        /// Remove a card from hand
        /// </summary>
        /// <param name="cardController"></param>
        public void RemoveCard(CardController cardController)
        {
            if (!_cards.Contains(cardController)) return;
            _cards.Remove(cardController);
            OnCardRemoved?.Invoke(cardController);
        }

        /// <summary>
        /// Add a card to the hand.
        /// </summary>
        /// <param name="cardController"></param>
        public void AddCard(CardController cardController)
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
            if (_cards.Count >= maxHandSize) return;
            if (!Deck.Draw(out CardInstance cardInstance)) return;
            var controller = CardFactory.Instance.Create(cardInstance);
            AddCard(controller);
        }

        public void DrawHand()
        {
            for (int i = 0; i < cardToDrawPerTurn; i++)
            {
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
        }

        public bool Has(CardController cardController)
        {
            return _cards.Contains(cardController);
        }
    }
}