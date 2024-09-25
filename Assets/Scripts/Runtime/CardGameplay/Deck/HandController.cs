using System;
using System.Collections.Generic;
using Runtime.CardGameplay.Card;
using Sirenix.OdinInspector;
using UnityEngine;
using Utilities;

namespace Runtime.CardGameplay.Deck
{
    public class HandController : Singleton<HandController>
    {
        public Deck Deck { get; set; }

        [ShowInInspector, ReadOnly] private List<CardController> _cards = new();

        public event Action<CardController> OnCardAdded;
        public event Action<CardController> OnCardRemoved;


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
            _cards.Add(cardController);
            OnCardAdded?.Invoke(cardController);
        }

        /// <summary>
        /// Draw a card from the deck, create for it a game object and add it to the hand
        /// </summary>
        [Button]
        public void DrawCard()
        {
            var cardInstance = Deck.Draw();
            var controller = CardFactory.Instance.Create(cardInstance);
            AddCard(controller);
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
    }
}