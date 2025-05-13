using System.Collections.Generic;
using DG.Tweening;
using Runtime.CardGameplay.Card;
using TMPro;
using UnityEngine;
using Utilities;

namespace Runtime.CardGameplay.Deck
{
    public class DeckView : MonoService<DeckView>
    {
        [SerializeField] private TextMeshProUGUI _drawPileCounter;
        [SerializeField] private TextMeshProUGUI _cardDrawCounter;
        [SerializeField] private TextMeshProUGUI _discardPileCounter;
        [SerializeField] private TextMeshProUGUI _consumePileCounter;
        [SerializeField] private Ease _ease;

        private Deck _deck;

        public void Setup(Deck deck)
        {
            _deck = deck;

            _drawPileCounter.text = "0";
            _discardPileCounter.text = "0";
            _cardDrawCounter.text = "0";

            var hand = ServiceLocator.Get<HandController>();

            _cardDrawCounter.text = "+" + hand.DrawPerTurn;


            _deck.OnDrawPileUpdated += pile => _drawPileCounter.text = pile.Count.ToString();
            _deck.OnDiscardPileUpdated += OnDeckOnOnDiscardPileUpdated;
            _deck.OnBurnPileUpdated += consumePile => consumePile.Count.ToString();

            hand.OnCardDrawPerTurnUpdated += (count) => _cardDrawCounter.text = "+" + count;
        }


        private void OnDeckOnOnDiscardPileUpdated(Stack<CardInstance> pile)
        {
            _discardPileCounter.text = pile.Count.ToString();
        }
    }
}