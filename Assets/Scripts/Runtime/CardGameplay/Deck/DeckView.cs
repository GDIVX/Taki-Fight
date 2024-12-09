using System.Collections.Generic;
using DG.Tweening;
using Runtime.CardGameplay.Card;
using TMPro;
using UnityEngine;

namespace Runtime.CardGameplay.Deck
{
    public class DeckView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _drawPileCounter;
        [SerializeField] private TextMeshProUGUI _discardPileCounter;
        [SerializeField] private Ease _ease;

        private Deck _deck;

        public void Setup(Deck deck)
        {
            _deck = deck;

            _deck.OnDrawPileUpdated += pile => _drawPileCounter.text = pile.Count.ToString();
            _deck.OnDiscardPileUpdated += OnDeckOnOnDiscardPileUpdated;
        }



        private void OnCardDiscarded(CardInstance cardInstance)
        {
            var controller = cardInstance.Controller;
            var view = controller.View;


        }

        private void OnDeckOnOnDiscardPileUpdated(Stack<CardInstance> pile)
        {
            _discardPileCounter.text = pile.Count.ToString();
        }
    }
}