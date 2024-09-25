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
        private int maxComboCount;

        [SerializeField, BoxGroup("Combo Color")]
        private Image imageToColorTransition;

        [SerializeField, BoxGroup("Combo Color")]
        private float colorTransitionDuration;

        [SerializeField, BoxGroup("Combo Color")]
        private Ease colorTransitionEase;

        [SerializeField, BoxGroup("Combo Message")]
        private TextMeshProUGUI comboText;

        private int _comboCount = 0;

        private void Awake()
        {
            BoardController.Instance.OnCardAdded += OnCardAdded;
            BoardController.Instance.OnCardRemoved += OnCardRemoved;
            BoardController.Instance.OnCardAddedExplainReason += UpdateComboText;
        }

        private void UpdateComboText(string reason)
        {
            comboText.text = reason;
        }

        protected override void OnCardAdded(CardController cardController)
        {
            base.OnCardAdded(cardController);
            UpdateComboColor();
        }


        protected override void OnCardRemoved(CardController cardController)
        {
            base.OnCardRemoved(cardController);
            comboText.text = "";
            UpdateComboColor();
        }

        private void UpdateComboColor()
        {
            _comboCount = BoardController.Instance.IsSequenceIsIntact()
                ? Mathf.Min(maxComboCount, BoardController.Instance.SequenceCount)
                : 0;
            var fraction = (float)_comboCount / (float)maxComboCount;
            Color color = Color.LerpUnclamped(minColor, maxColor, fraction);
            imageToColorTransition.DOColor(color, colorTransitionDuration);
        }
    }
}