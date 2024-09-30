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

        // [SerializeField, BoxGroup("Combo Color")]
        // private int maxComboCount;

        [SerializeField, BoxGroup("Combo Color")]
        private Image imageToColorTransition;

        [SerializeField, BoxGroup("Combo Color")]
        private float colorTransitionDuration;

        [SerializeField, BoxGroup("Combo Color")]
        private Ease colorTransitionEase;

        // [SerializeField, BoxGroup("Combo Counter")]
        // private TextMeshProUGUI comboCounter;

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

        private int _comboCount = 0;

        private void Awake()
        {
            BoardController.Instance.OnCardAdded += OnCardAdded;
            BoardController.Instance.OnCardRemoved += OnCardRemoved;
            BoardController.Instance.OnCardSetAside += OnCardSetAside;
            BoardController.Instance.OnMatchValuesChanged += OnMatchValuesChanged;


            // UpdateComboColor();
        }

        private void OnCardSetAside(CardController card)
        {
            //Remove the card
            Cards.Remove(card);

            //Shrink and move the card to fit with the current color image 
            card.transform.DOMove(setAsideDestination.transform.position, setAsideDuration).SetEase(setAsideEase);
            card.transform.DOScale(setAsideDestination.localScale, setAsideDuration).SetEase(setAsideEase).onComplete +=
                () =>
                {
                    //disable the view and then reset for future use
                    card.Disable();
                    card.View.ReturnToDefault();
                };
        }

        private void OnMatchValuesChanged(Suit suit, int number)
        {
            currentColorImage.DOColor(suitColorPallet.GetColor(suit), colorTransitionDuration)
                .SetEase(colorTransitionEase);
            currentNumberText.text = number.ToString();
        }


        // private void UpdateComboColor()
        // {
        //     _comboCount = BoardController.Instance.Combo;
        //     var fraction = (float)_comboCount / (float)maxComboCount;
        //     Color color = Color.LerpUnclamped(minColor, maxColor, fraction);
        //     imageToColorTransition.DOColor(color, colorTransitionDuration);
        // }
    }
}