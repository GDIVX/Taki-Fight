﻿using System;
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
        [SerializeField, Required, BoxGroup("Dependencies")]
        private BoardController _boardController;

        [SerializeField, BoxGroup("Combo Color")]
        private float colorTransitionDuration;

        [SerializeField, BoxGroup("Combo Color")]
        private Ease colorTransitionEase;

        [SerializeField, BoxGroup("Values Indicator")]
        private TextMeshProUGUI currentRankText;

        [SerializeField, BoxGroup("Values Indicator")]
        private Image currentSuiteImage;

        [SerializeField, BoxGroup("Values Indicator")]
        private SuitColorPallet suitColorPallet;

        [SerializeField, BoxGroup("Energy")] private TextMeshProUGUI energyCountText;

        private void Awake()
        {
            SubscribeToBoardControllerEvents();
            OnMatchCountChanged(0);
        }

        private void OnDestroy()
        {
            UnsubscribeFromBoardControllerEvents();
        }

        private void SubscribeToBoardControllerEvents()
        {
            if (_boardController == null) return;
            _boardController.OnMatchValuesChanged += OnMatchValuesChanged;
            _boardController.RegisterToMatchCountChanged(OnMatchCountChanged);
        }


        private void UnsubscribeFromBoardControllerEvents()
        {
            if (_boardController == null) return;
            _boardController.OnMatchValuesChanged -= OnMatchValuesChanged;
            _boardController.UnregisterToMatchCountChanged(OnMatchCountChanged);
        }

        private void OnMatchCountChanged(int count)
        {
            energyCountText.text = count.ToString();
        }

        private void OnMatchValuesChanged(Suit suit, int number)
        {
            currentRankText.color = suitColorPallet.GetColor(suit == Suit.White ? Suit.Black : Suit.White);
            Color targetColor = suitColorPallet.GetColor(suit);
            currentSuiteImage.DOColor(targetColor, colorTransitionDuration).SetEase(colorTransitionEase);
            currentRankText.text = number.ToString();
        }
    }
}