using System.Collections.Generic;
using DG.Tweening;
using Runtime.CardGameplay.Card;
using Sirenix.OdinInspector;
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


        private CardController _hoveredCard;
        private CardController _selectedCard;
        private bool _isSwapping;
        private Sequence _currentArrangeSequence;

        private void Awake()
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

        private void OnDisable()
        {
            if (_currentArrangeSequence != null && _currentArrangeSequence.IsActive())
            {
                _currentArrangeSequence.Kill();
            }
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

            // Animate the swap
            Sequence swapSequence = DOTween.Sequence();
            swapSequence.Join(first.Transform.DOLocalMove(second.Transform.localPosition, animationDuration)
                .SetEase(easeType));
            swapSequence.Join(second.Transform.DOLocalMove(first.Transform.localPosition, animationDuration)
                .SetEase(easeType));
            swapSequence.OnComplete(() =>
            {
                first.Transform.SetSiblingIndex(secondIndex);
                second.Transform.SetSiblingIndex(firstIndex);
                _isSwapping = false;
            });
        }

        private int GetIndex(CardController card) => card.Transform.GetSiblingIndex();

        [Button]
        public void ArrangeCardsInArch()
        {
            int count = _cards.Count;

            // Stop any existing animation and reset transforms
            if (_currentArrangeSequence != null && _currentArrangeSequence.IsPlaying())
            {
                _currentArrangeSequence.Kill();
                foreach (var c in _cards)
                    c.ViewMediator.ResetLayoutState(); // only resets transform, not sibling
            }

            // Disable hover during layout
            foreach (var c in _cards)
                c.ViewMediator.SetBlockRaycast(false);

            // Keep hovered card on top
            _hoveredCard?.Transform.SetAsLastSibling();

            // Single-card shortcut
            if (count == 1)
            {
                var single = _cards[0];
                single.ViewMediator
                    .SetRoot(Vector3.zero, Vector3.zero, animationDuration)
                    .OnComplete(() => single.ViewMediator.SetBlockRaycast(true));
                return;
            }

            if (count == 0 || arcAngle == 0f) return;

            // Compute arc radius
            float baseWidth = _rectTransform.rect.width;
            float scaledWidth = Mathf.Lerp(
                baseWidth * minArcWidthFactor,
                baseWidth * maxArcWidthFactor,
                Mathf.InverseLerp(1f, 10f, count)
            );
            float radius = scaledWidth / (2f * Mathf.Tan(Mathf.Deg2Rad * (arcAngle / 2f)));
            if (float.IsNaN(radius) || float.IsInfinity(radius)) return;

            // Build new sequence
            _currentArrangeSequence = DOTween.Sequence();

            // Join all card animations (position + rotation)
            for (int i = 0; i < count; i++)
            {
                float t = (count > 1) ? i / (float)(count - 1) : 0f;
                float angle = Mathf.Lerp(-arcAngle / 2f, arcAngle / 2f, t);
                Vector3 pos = new Vector3(
                    Mathf.Sin(Mathf.Deg2Rad * angle) * radius,
                    Mathf.Cos(Mathf.Deg2Rad * angle) * radius - radius,
                    0f
                );
                Vector3 rot = new Vector3(0f, 0f, -angle);

                _currentArrangeSequence.Join(
                    _cards[i].ViewMediator.SetRoot(pos, rot, animationDuration)
                );
            }

            // Reorder siblings once, at the very end
            _currentArrangeSequence
                .AppendCallback(() =>
                {
                    for (int i = 0; i < _cards.Count; i++)
                    {
                        if (_cards[i] != _hoveredCard)
                            _cards[i].Transform.SetSiblingIndex(i);
                    }
                })
                .AppendInterval(0f) // ensure it's applied in the final frame
                .OnComplete(() =>
                {
                    foreach (var c in _cards)
                    {
                        c.ViewMediator.SetBlockRaycast(true);
                    }
                });
        }
    }
}