﻿using System.Collections.Generic;
using DG.Tweening;
using Runtime.CardGameplay.Card;
using UnityEngine;
using UnityEngine.Events;

namespace Runtime.CardGameplay.Deck
{
    [RequireComponent(typeof(RectTransform))]
    public class HorizontalCardListView : MonoBehaviour
    {
        [SerializeField] private float arcAngle = 30f;
        [SerializeField] private float animationDuration = 0.5f;
        [SerializeField] private Ease easeType = Ease.InOutSine;
        [SerializeField] private float minArcWidthFactor = 0.2f;
        [SerializeField] private float maxArcWidthFactor = 1f;

        private RectTransform _rectTransform;
        private readonly List<CardController> _cards = new();

        public UnityEvent<CardController> PointerEnterEvent;
        public UnityEvent<CardController> PointerExitEvent;
        public UnityEvent<CardController> BeginDragEvent;
        public UnityEvent<CardController> EndDragEvent;

        private CardController _hoveredCard;
        private CardController _selectedCard;
        private bool _isSwapping;
        private Sequence _currentArrangeSequence;

        private void Start()
        {
            _rectTransform = GetComponent<RectTransform>();
        }

        public void AddCard(CardController card)
        {
            card.Transform.SetParent(transform);
            _cards.Add(card);
            ArrangeCardsInArch();
        }

        public void RemoveCard(CardController card)
        {
            _cards.Remove(card);
            ArrangeCardsInArch();
        }

        private void EnsureCardOnTop(CardController card)
        {
            if (card != null)
            {
                // Kill any existing arrangement animation
                if (_currentArrangeSequence != null && _currentArrangeSequence.IsPlaying())
                {
                    _currentArrangeSequence.Kill();
                }
                card.Transform.SetAsLastSibling();
            }
        }

        private void OnCardPointerEnter(CardController card)
        {
            _hoveredCard = card;
            EnsureCardOnTop(card);
            PointerEnterEvent?.Invoke(card);
        }

        private void OnCardPointerExit(CardController card)
        {
            if (_hoveredCard == card) _hoveredCard = null;
            PointerExitEvent?.Invoke(card);
        }

        private void OnBeginDrag(CardController card)
        {
            _selectedCard = card;
            BeginDragEvent?.Invoke(card);
        }

        private void OnEndDrag(CardController card)
        {
            if (_selectedCard == card) _selectedCard = null;
            EndDragEvent?.Invoke(card);
        }

        private void Update()
        {
            if (_selectedCard == null || _isSwapping) return;
            
            for (int i = 0; i < _cards.Count; i++)
            {
                var other = _cards[i];
                if (_selectedCard == other) continue;
                if (_selectedCard.Transform.position.x > other.Transform.position.x &&
                    GetIndex(_selectedCard) < GetIndex(other))
                {
                    Swap(_selectedCard, other);
                    break;
                }
                else if (_selectedCard.Transform.position.x < other.Transform.position.x &&
                         GetIndex(_selectedCard) > GetIndex(other))
                {
                    Swap(_selectedCard, other);
                    break;
                }
            }
        }

        private void Swap(CardController first, CardController second)
        {
            _isSwapping = true;
            int firstIndex = GetIndex(first);
            int secondIndex = GetIndex(second);
            first.Transform.SetSiblingIndex(secondIndex);
            second.Transform.SetSiblingIndex(firstIndex);
            _isSwapping = false;
        }

        private int GetIndex(CardController card) => card.Transform.GetSiblingIndex();

        public void ArrangeCardsInArch()
        {
            int count = _cards.Count;
            if (count == 0 || arcAngle == 0) return;
            float baseWidth = _rectTransform.rect.width;

            float scaledWidth = Mathf.Lerp(
                baseWidth * minArcWidthFactor,
                baseWidth * maxArcWidthFactor,
                Mathf.InverseLerp(1, 10, count)
            );

            float radius = scaledWidth / (2 * Mathf.Tan(Mathf.Deg2Rad * (arcAngle / 2)));
            if (float.IsNaN(radius) || float.IsInfinity(radius)) return;

            // Kill any existing arrangement animation
            if (_currentArrangeSequence != null && _currentArrangeSequence.IsPlaying())
            {
                _currentArrangeSequence.Kill();
            }

            _currentArrangeSequence = DOTween.Sequence();

            for (int i = 0; i < count; i++)
            {
                float angle = Mathf.Lerp(-arcAngle / 2, arcAngle / 2, i / Mathf.Max(1f, (count - 1f)));
                float xPos = Mathf.Sin(Mathf.Deg2Rad * angle) * radius;
                float yPos = Mathf.Cos(Mathf.Deg2Rad * angle) * radius - radius;

                var card = _cards[i];
                
                if (card != _hoveredCard)
                {
                    card.Transform.SetSiblingIndex(i);
                }

                _currentArrangeSequence.Join(
                    card.Transform.DOLocalMove(new Vector3(xPos, yPos, 0), animationDuration)
                        .SetEase(easeType)
                );
                _currentArrangeSequence.Join(
                    card.Transform.DOLocalRotate(new Vector3(0, 0, -angle), animationDuration)
                        .SetEase(easeType)
                );
            }

            _currentArrangeSequence.OnComplete(() => {
                foreach (var card in _cards)
                {
                    card.View.SetOriginalValues();
                }
                if (_hoveredCard != null)
                {
                    EnsureCardOnTop(_hoveredCard);
                }
            });
        }
    }
}