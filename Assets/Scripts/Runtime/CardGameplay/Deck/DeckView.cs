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

            //Shrink and move the card to fit with the image 
            controller.transform.DOMove(discardToLocation.position, discardAnimationDuration).SetEase(ease);
            controller.transform.DOScale(discardToLocation.localScale, discardAnimationDuration).SetEase(ease)
                    .onComplete +=
                () =>
                {
                    //disable the view and then reset for future use
                    controller.Disable();
                    controller.View.ReturnToDefault();
                };
        }

        private void OnDeckOnOnDiscardPileUpdated(Stack<CardInstance> pile)
        {
            discardPileCounter.text = pile.Count.ToString();
        }
    }
}