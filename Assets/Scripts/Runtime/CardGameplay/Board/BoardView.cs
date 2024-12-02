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
        [SerializeField, Required, BoxGroup("Dependencies")]
        private BoardController _boardController;

        [SerializeField, BoxGroup("Combo Color")]
        private float _colorTransitionDuration;

        [SerializeField, BoxGroup("Combo Color")]
        private Ease _colorTransitionEase;

        [SerializeField, BoxGroup("Values Indicator")]
        private TextMeshProUGUI _currentRankText;

        [SerializeField, BoxGroup("Values Indicator")]
        private Image _currentSuiteImage;

        [SerializeField, BoxGroup("Values Indicator")]
        private SuitColorPallet _suitColorPallet;

        [SerializeField, BoxGroup("Energy")] private TextMeshProUGUI _energyCountText;

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
            _energyCountText.text = count.ToString();
        }

        private void OnMatchValuesChanged(Suit suit, int number)
        {
            _currentRankText.color = _suitColorPallet.GetColor(suit == Suit.White ? Suit.Black : Suit.White);
            _currentRankText.gameObject.SetActive(suit != Suit.Black && suit != Suit.White);
            Color targetColor = _suitColorPallet.GetColor(suit);
            _currentSuiteImage.DOColor(targetColor, _colorTransitionDuration).SetEase(_colorTransitionEase);
            _currentRankText.text = number.ToString();
        }
    }
}