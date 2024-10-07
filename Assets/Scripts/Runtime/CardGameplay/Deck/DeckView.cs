using System.Collections.Generic;
using DG.Tweening;
using Runtime.CardGameplay.Card;
using TMPro;
using UnityEngine;

namespace Runtime.CardGameplay.Deck
{
    public class DeckView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI drawPileCounter;
        [SerializeField] private TextMeshProUGUI discardPileCounter;

        [SerializeField] private Transform discardToLocation;
        [SerializeField] private float discardAnimationDuration;
        [SerializeField] private Ease ease;

        private Deck _deck;

        public void Setup(Deck deck)
        {
            _deck = deck;

            _deck.OnDrawPileUpdated += pile => drawPileCounter.text = pile.Count.ToString();
            _deck.OnDiscardPileUpdated += OnDeckOnOnDiscardPileUpdated;
            _deck.OnCardDiscarded += OnCardDiscarded;
        }

        private void OnCardDiscarded(CardInstance cardInstance)
        {
            var controller = cardInstance.Controller;

            // Use the CardView's animation services for discarding
            controller.View.AnimateToPosition(discardToLocation.position, discardAnimationDuration, ease);
            controller.View.AnimateToScale(discardToLocation.localScale, discardAnimationDuration, ease)
                .OnComplete(() =>
                {
                    // Disable the view and then reset for future use
                    controller.Disable();
                    controller.View.AnimateReturnToDefault();
                });
        }

        private void OnDeckOnOnDiscardPileUpdated(Stack<CardInstance> pile)
        {
            discardPileCounter.text = pile.Count.ToString();
        }
    }
}