using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Runtime.CardGameplay.Card;
using Runtime.CardGameplay.Card.View;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Runtime.CardGameplay.Deck
{
    public class HorizontalCardListView : MonoBehaviour
    {
        [SerializeField] private float _arcAngle = 30f;
        [SerializeField] private float _animationDuration = 0.5f;
        [SerializeField] private Ease _easeType = Ease.InOutSine;

        [SerializeField] private float _minArcWidthFactor = 0.2f;
        [SerializeField] private float _maxArcWidthFactor = 1f;

        [ShowInInspector, ReadOnly] protected readonly List<CardController> Cards = new();

        private RectTransform _rectTransform;

        protected virtual void OnCardAdded(CardController cardController)
        {
            cardController.Transform.SetParent(transform);
            Cards.Add(cardController);
            ArrangeCardsInArch();
        }

        protected virtual void OnCardRemoved(CardController cardController)
        {
            Cards.Remove(cardController);
            ArrangeCardsInArch();
        }

        [Button]
        protected void ArrangeCardsInArch()
        {
            int cardCount = Cards.Count;
            if (cardCount == 0 || _arcAngle == 0) return;

            _rectTransform ??= GetComponent<RectTransform>();
            float baseWidth = _rectTransform.rect.width;

            // Dynamically adjust the arc width factor based on the number of cards
            float scaledWidth = Mathf.Lerp(baseWidth * _minArcWidthFactor, baseWidth * _maxArcWidthFactor,
                Mathf.InverseLerp(1, 10, cardCount));

            float radius = scaledWidth / (2 * Mathf.Tan(Mathf.Deg2Rad * (_arcAngle / 2)));
            if (float.IsNaN(radius) || float.IsInfinity(radius)) return;

            for (int i = 0; i < cardCount; i++)
            {
                float angle = Mathf.Lerp(-_arcAngle / 2, _arcAngle / 2, i / Mathf.Max(1, (float)(cardCount - 1)));
                float xPos = Mathf.Sin(Mathf.Deg2Rad * angle) * radius;
                float yPos = Mathf.Cos(Mathf.Deg2Rad * angle) * radius - radius;

                if (float.IsNaN(xPos) || float.IsNaN(yPos)) continue;

                var card = Cards[i];
                card.Transform.SetSiblingIndex(i);
                card.Transform.DOLocalMove(new Vector3(xPos, yPos, 0), _animationDuration).SetEase(_easeType);
                card.Transform.DOLocalRotate(new Vector3(0, 0, -angle), _animationDuration).SetEase(_easeType)
                    .onComplete += () => StartCoroutine(WaitAndSetViewNewValues(card.View));
                //ensure correct ordering 
            }
        }

        private IEnumerator WaitAndSetViewNewValues(CardView view)
        {
            yield return new WaitForSeconds(_animationDuration);
            view.SetOriginalValues();
        }
    }
}