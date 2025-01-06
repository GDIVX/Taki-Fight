using System;
using System.Collections.Generic;
using System.Linq;
using Runtime.CardGameplay.Card;
using Runtime.CardGameplay.SlotMachineLib;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Runtime.CardGameplay.Deck
{
    public class CardCollection : MonoBehaviour
    {
        [SerializeField, TableList] private List<CardInstance> _cards;
        [SerializeField] private int _sizeLimitMin, _sizeLimitMax;
        [SerializeField] private StarterDeck _starterDeck;

        [SerializeField, TabGroup("Dependencies")]
        private DeckView _deckView;

        [SerializeField, Required, TabGroup("Dependencies")]
        private HandController _handController;

        [ShowInInspector, ReadOnly] public Deck Deck { get; private set; }
        public ReelDefinition ReelDefinition { get; private set; }

        public void Init()
        {
            _cards = _starterDeck.Cards.Select(data => new CardInstance(data)).ToList();
            Deck = new Deck(_cards);
            _deckView.Setup(Deck);
            ReelDefinition = _starterDeck.ReelDefinition;
        }

        public bool TryAddCard(CardInstance card)
        {
            if (_cards.Count >= _sizeLimitMax)
            {
                Debug.LogWarning("Failed to add a card to the collection due to exceeding the size limit");
                return false;
            }

            _cards.Add(card);
            return true;
        }

        public bool TryRemoveCard(CardInstance card)
        {
            if (_cards.Count <= _sizeLimitMin)
            {
                Debug.LogWarning("Failed to remove a card from collection as doing so would make the deck too small");
                return false;
            }

            if (!_cards.Contains(card))
            {
                Debug.LogError($"Trying to remove card {card} but it dose not exist in the collection");
                return false;
            }

            _cards.Remove(card);
            return true;
        }

        [Button]
        public Deck CreateDeck()
        {
            Deck.Setup(_cards);
            _handController.Deck = Deck;
            return Deck;
        }
    }
}