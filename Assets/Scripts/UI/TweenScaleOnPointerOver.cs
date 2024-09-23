using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI
{
    public class TweenScaleOnPointerOver : MonoBehaviour, IPointerExitHandler, IPointerEnterHandler
    {
        [SerializeField] private float scaleTime = 1;
        [SerializeField] private float scaleFactor;

        private Vector3 _originalScale;

        public void OnPointerEnter(PointerEventData eventData)
        {
            var scaleTarget = transform.localScale * scaleFactor;
            _originalScale = transform.localScale;
            transform.DOScale(scaleTarget, scaleTime).SetEase(Ease.InOutElastic);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            transform.DOScale(_originalScale, scaleTime).SetEase(Ease.InOutElastic);
        }
    }
}