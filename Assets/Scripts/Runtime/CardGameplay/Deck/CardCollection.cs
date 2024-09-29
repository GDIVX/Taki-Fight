using System;
using System.Collections.Generic;
using Runtime.CardGameplay.Card;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Runtime.CardGameplay.Deck
{
    public class CardCollection : MonoBehaviour
    {
        [SerializeField, TableList] private List<CardInstance> cards;
        [SerializeField] private int sizeLimitMin, sizeLimitMax;
        [SerializeField] private StarterDeck starterDeck;

        [SerializeField, TabGroup("Dependencies")]
        private DeckView deckView;

        [ShowInInspector, ReadOnly] public Deck Deck { get; private set; }

        private void Start()
        {
            cards = starterDeck.Clone();
            Deck = new Deck(cards);
            deckView.Setup(Deck);
        }

        public bool TryAddCard(CardInstance card)
        {
            if (cards.Count >= sizeLimitMax)
            {
                Debug.LogWarning("Failed to add a card to the collection due to exceeding the size limit");
                return false;
            }

            cards.Add(card);
            return true;
        }

        public bool TryRemoveCard(CardInstance card)
        {
            if (cards.Count <= sizeLimitMin)
            {
                Debug.LogWarning("Failed to remove a card from collection as doing so would make the deck too small");
                return false;
            }

            if (!cards.Contains(card))
            {
                Debug.LogError($"Trying to remove card {card} but it dose not exist in the collection");
                return false;
            }

            cards.Remove(card);
            return true;
        }

        [Button]
        public Deck CreateDeck()
        {
            Deck.Setup(cards);
            HandController.Instance.Deck = Deck;
            return Deck;
        }
    }
}