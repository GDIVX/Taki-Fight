using System.Collections.Generic;
using DG.Tweening;
using Runtime.CardGameplay.Card;
using Sirenix.OdinInspector;
using UnityEngine;
using Utilities;

namespace Runtime.CardGameplay.Deck
{
    public class HandView : Singleton<HandView>
    {
        [SerializeField] private float arcAngle = 30f;
        [SerializeField] private float animationDuration = 0.5f;
        [SerializeField] private Ease easeType = Ease.InOutSine;

        [SerializeField] private float minArcWidthFactor = 0.2f;
        [SerializeField] private float maxArcWidthFactor = 1f;

        private readonly List<CardController> _cards = new();

        private void Awake()
        {
            // Subscribe to events for card addition and removal if needed
            HandController.Instance.OnCardAdded += OnCardAdded;
            HandController.Instance.OnCardRemoved += OnCardRemoved;
        }


        private void OnCardAdded(CardController cardController)
        {
            cardController.transform.SetParent(transform);
            _cards.Add(cardController);
            ArrangeCardsInArch();
        }

        private void OnCardRemoved(CardController cardController)
        {
            _cards.Remove(cardController);
            ArrangeCardsInArch();
        }

        [Button]
        private void ArrangeCardsInArch()
        {
            int cardCount = _cards.Count;
            if (cardCount == 0 || arcAngle == 0) return;

            RectTransform rectTransform = GetComponent<RectTransform>();
            float baseWidth = rectTransform.rect.width;

            // Dynamically adjust the arc width factor based on the number of cards
            float scaledWidth = Mathf.Lerp(baseWidth * minArcWidthFactor, baseWidth * maxArcWidthFactor,
                Mathf.InverseLerp(1, 10, cardCount));

            float radius = scaledWidth / (2 * Mathf.Tan(Mathf.Deg2Rad * (arcAngle / 2)));
            if (float.IsNaN(radius) || float.IsInfinity(radius)) return;

            for (int i = 0; i < cardCount; i++)
            {
                float angle = Mathf.Lerp(-arcAngle / 2, arcAngle / 2, i / Mathf.Max(1, (float)(cardCount - 1)));
                float xPos = Mathf.Sin(Mathf.Deg2Rad * angle) * radius;
                float yPos = Mathf.Cos(Mathf.Deg2Rad * angle) * radius - radius;

                if (float.IsNaN(xPos) || float.IsNaN(yPos)) continue;

                _cards[i].transform.DOLocalMove(new Vector3(xPos, yPos, 0), animationDuration).SetEase(easeType);
                _cards[i].transform.DOLocalRotate(new Vector3(0, 0, -angle), animationDuration).SetEase(easeType);
            }
        }
    }
}