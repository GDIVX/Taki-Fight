using System;
using System.Collections.Generic;
using DG.Tweening;
using Runtime.CardGameplay.Card;
using Runtime.CardGameplay.Deck;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Runtime.CardGameplay.Board
{
    public class BoardView : HorizontalCardListView
    {
        [SerializeField, BoxGroup("Combo Color")]
        private Color minColor, maxColor;

        [SerializeField, BoxGroup("Combo Color")]
        private float colorTransitionDuration;

        [SerializeField, BoxGroup("Combo Color")]
        private Ease colorTransitionEase;

        [SerializeField, BoxGroup("Values Indicator")]
        private TextMeshProUGUI currentNumberText;

        [SerializeField, BoxGroup("Values Indicator")]
        private Image currentColorImage;

        [SerializeField, BoxGroup("Values Indicator")]
        private SuitColorPallet suitColorPallet;

        [SerializeField, BoxGroup("Set Aside")]
        private Transform setAsideDestination;

        [SerializeField, BoxGroup("Set Aside")]
        private float setAsideDuration;

        [SerializeField, BoxGroup("Set Aside")]
        private Ease setAsideEase;

        private void Awake()
        {
            SubscribeToBoardControllerEvents();
        }

        private void OnDestroy()
        {
            UnsubscribeFromBoardControllerEvents();
        }

        private void SubscribeToBoardControllerEvents()
        {
            if (BoardController.Instance != null)
            {
                BoardController.Instance.OnCardAdded += OnCardAdded;
                BoardController.Instance.OnCardRemoved += OnCardRemoved;
                BoardController.Instance.OnCardSetAside += OnCardSetAside;
                BoardController.Instance.OnMatchValuesChanged += OnMatchValuesChanged;
            }
        }

        private void UnsubscribeFromBoardControllerEvents()
        {
            if (BoardController.Instance != null)
            {
                BoardController.Instance.OnCardAdded -= OnCardAdded;
                BoardController.Instance.OnCardRemoved -= OnCardRemoved;
                BoardController.Instance.OnCardSetAside -= OnCardSetAside;
                BoardController.Instance.OnMatchValuesChanged -= OnMatchValuesChanged;
            }
        }

        private void OnCardSetAside(CardController card)
        {
            if (card == null) return;

            // Remove the card
            Cards.Remove(card);

            // Use the CardView's animation services for setting aside
            card.View.AnimateToPosition(setAsideDestination.position, setAsideDuration, setAsideEase);
            card.View.AnimateToScale(setAsideDestination.localScale, setAsideDuration, setAsideEase)
                .OnComplete(() =>
                {
                    // Disable the view and then reset for future use
                    card.Disable();
                    card.View.AnimateReturnToDefault();
                });
        }

        // protected override void OnCardRemoved(CardController card)
        // {
        //     if (card == null || !Cards.Contains(card)) return;
        //
        //     Cards.Remove(card);
        //     card.Disable(); 
        // }

        private void OnMatchValuesChanged(Suit suit, int number)
        {
            Color targetColor = suitColorPallet.GetColor(suit);
            currentColorImage.DOColor(targetColor, colorTransitionDuration).SetEase(colorTransitionEase);
            currentNumberText.text = number.ToString();
        }
    }
}