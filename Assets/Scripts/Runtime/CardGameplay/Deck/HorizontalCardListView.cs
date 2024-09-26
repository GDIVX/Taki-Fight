using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Runtime.CardGameplay.Card;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Runtime.CardGameplay.Deck
{
    public class HorizontalCardListView : MonoBehaviour
    {
        [SerializeField] private float arcAngle = 30f;
        [SerializeField] private float animationDuration = 0.5f;
        [SerializeField] private Ease easeType = Ease.InOutSine;

        [SerializeField] private float minArcWidthFactor = 0.2f;
        [SerializeField] private float maxArcWidthFactor = 1f;

        [ShowInInspector, ReadOnly] protected readonly List<CardController> Cards = new();

        protected virtual void OnCardAdded(CardController cardController)
        {
            cardController.transform.SetParent(transform);
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

                Cards[i].transform.DOLocalMove(new Vector3(xPos, yPos, 0), animationDuration).SetEase(easeType);
                Cards[i].transform.DOLocalRotate(new Vector3(0, 0, -angle), animationDuration).SetEase(easeType);

                StartCoroutine(WaitAndSetViewNewValues(Cards[i].View));
            }
        }

        private IEnumerator WaitAndSetViewNewValues(CardView view)
        {
            yield return new WaitForSeconds(animationDuration);
            view.SetOriginalValues();
        }
    }
}