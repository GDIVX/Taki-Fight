using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Runtime.CardGameplay.Card;
using Sirenix.OdinInspector;
using UnityEngine;
using Utilities;

namespace Runtime.CardGameplay.Deck
{
    public class HandController : MonoBehaviour, IHandController
    {
        [SerializeField] private int _cardToDrawPerTurn;
        [SerializeField] private int _maxHandSize;
        [SerializeField, Required] private CardFactory _cardFactory;
        public Deck Deck { get; set; }
        [ShowInInspector, ReadOnly] private List<ICardController> _cards = new();

        public event Action<ICardController> OnCardAdded;
        public event Action<ICardController> OnCardRemoved;

        public int CardToDrawPerTurn
        {
            get => _cardToDrawPerTurn;
            set => _cardToDrawPerTurn = value;
        }

        /// <summary>
        /// Remove a card from hand
        /// </summary>
        /// <param name="cardController"></param>
        private void RemoveCard(ICardController cardController)
        {
            if (!_cards.Contains(cardController)) return;
            _cards.Remove(cardController);
            OnCardRemoved?.Invoke(cardController);
        }

        /// <summary>
        /// Add a card to the hand.
        /// </summary>
        /// <param name="cardController"></param>
        private void AddCard(ICardController cardController)
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
            var controller = _cardFactory.Create(cardInstance);
            AddCard(controller);
        }

        public void DrawHand()
        {
            for (int i = 0; i < _cardToDrawPerTurn; i++)
            {
                DrawCard();
            }
        }

        /// <summary>
        /// Discard a card to the deck's discard pile, and remove it from hand
        /// </summary>
        /// <param name="cardController"></param>
        [Button]
        public void DiscardCard(ICardController cardController)
        {
            //If the card is already discard, return to avoid side effects
            if (Deck.IsDiscarded(cardController.Instance)) return;

            Deck.Discard(cardController.Instance);
            RemoveCard(cardController);
        }

        public bool Has(ICardController cardController)
        {
            return _cards.Contains(cardController);
        }
    }
}