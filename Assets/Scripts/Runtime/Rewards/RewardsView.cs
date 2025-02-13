using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Runtime.CardGameplay.Card;
using Runtime.CardGameplay.Card.View;
using UnityEngine;
using UnityEngine.UI;

namespace Runtime.Rewards
{
    public class RewardsView : MonoBehaviour
    {
        [SerializeField] private Transform _cardContainer;
        [SerializeField] private RewardCardView _cardPrefab;
        [SerializeField] private Button _skipButton;
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private RectTransform _windowTransform;

        [Header("Animation Settings")] 
        [SerializeField] private float _windowFadeDuration = 0.3f;
        [SerializeField] private float _windowScaleDuration = 0.3f;
        [SerializeField] private float _cardAppearDelay = 0.1f;
        [SerializeField] private float _cardAppearScale = 1.2f;
        [SerializeField] private Ease _windowEase = Ease.OutBack;
        [SerializeField] private Ease _cardEase = Ease.OutQuad;

        private Action<CardData> _onRewardChosen;

        // Object pool for card views
        private readonly List<RewardCardView> _cardPool = new();

        private void Awake()
        {
            _windowTransform.gameObject.SetActive(false);
            _canvasGroup.alpha = 0;
            _windowTransform.localScale = Vector3.zero;
        }

        public void ShowCardRewardSelection(List<CardData> cards, Action<CardData> onSelect, Action onSkip)
        {
            _onRewardChosen = onSelect;

            // Reuse or instantiate card views
            PrepareCardViews(cards);

            StartCoroutine(ShowRewardsWithDelay(cards, onSkip));
        }

        private IEnumerator ShowRewardsWithDelay(List<CardData> cards, Action onSkip)
        {
            _windowTransform.gameObject.SetActive(true);
            yield return null; // Wait a frame for ContentSizeFitter to update

            // Animate window appearance
            _canvasGroup.DOFade(1, _windowFadeDuration);
            _windowTransform.DOScale(Vector3.one, _windowScaleDuration).SetEase(_windowEase);

            // Animate card appearances
            for (int i = 0; i < cards.Count; i++)
            {
                var cardView = _cardPool[i];
                cardView.transform.localScale = Vector3.zero;
                cardView.gameObject.SetActive(true);

                // Animate card appearance
                cardView.transform.DOScale(_cardAppearScale, _cardAppearDelay)
                    .SetEase(_cardEase)
                    .SetDelay(i * _cardAppearDelay)
                    .OnComplete(() => cardView.transform.DOScale(1f, 0.1f))
                    .OnComplete(() => cardView.IsHoverable = true);
            }

            // Setup skip button
            _skipButton.onClick.RemoveAllListeners();
            _skipButton.onClick.AddListener(() => CloseAndSkip(onSkip));
        }

        private void SelectCard(RewardCardView selectedCard)
        {
            _onRewardChosen?.Invoke(selectedCard.GetCardData());
            CloseView();
        }

        private void CloseAndSkip(Action onSkip)
        {
            onSkip?.Invoke();
            CloseView();
        }

        private void CloseView()
        {
            _canvasGroup.DOFade(0, _windowFadeDuration);
            _windowTransform.DOScale(Vector3.zero, _windowScaleDuration)
                .SetEase(_windowEase)
                .OnComplete(() => _windowTransform.gameObject.SetActive(false));

            // Deactivate all cards after closing
            foreach (var card in _cardPool)
            {
                card.gameObject.SetActive(false);
                card.IsHoverable = false;
            }
        }

        private void PrepareCardViews(List<CardData> cards)
        {
            // Activate or create the necessary card views
            for (int i = 0; i < cards.Count; i++)
            {
                RewardCardView cardView;
                if (i < _cardPool.Count)
                {
                    cardView = _cardPool[i];
                }
                else
                {
                    cardView = Instantiate(_cardPrefab, _cardContainer);
                    _cardPool.Add(cardView);
                }

                cardView.Init(cards[i], SelectCard);
                cardView.gameObject.SetActive(true);
            }

            // Deactivate any extra cards beyond the current need
            for (int i = cards.Count; i < _cardPool.Count; i++)
            {
                _cardPool[i].gameObject.SetActive(false);
            }
        }
    }
}
