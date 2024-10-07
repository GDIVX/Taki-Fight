using System;
using DG.Tweening;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Runtime.CardGameplay.Card
{
    public class CardView : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField, TabGroup("Draw")] private TextMeshProUGUI title;
        [SerializeField, TabGroup("Draw")] private TextMeshProUGUI description;
        [SerializeField, TabGroup("Draw")] private TextMeshProUGUI numberText;
        [SerializeField, TabGroup("Draw")] private Image image;
        [SerializeField, TabGroup("Draw")] private Image suitImage;
        [SerializeField, TabGroup("Draw")] private SuitColorPallet colorPallet;

        [SerializeField, TabGroup("Hover Animation")]
        private float hoverScaleFactor = 1.2f;

        [SerializeField, TabGroup("Hover Animation")]
        private float hoverRotationDuration = 0.3f;

        [SerializeField, TabGroup("Hover Animation")]
        private Ease hoverEaseType = Ease.OutQuad;

        [ShowInInspector, ReadOnly] private CardData _cardData;

        private Vector3 _originalScale;
        private Vector3 _originalPosition;
        private Vector3 _originalRotation;
        private int _originalSiblingIndex;

        private Tween _currentTween;
        private bool _isHovered;

        private void Awake()
        {
            _originalScale = transform.localScale;
            SetOriginalValues();
        }

        [Button]
        public void Draw(CardData data, int number, Suit suit = Suit.Default)
        {
            if (suit == Suit.Default)
            {
                suit = data.Suit;
            }

            if (suit == Suit.White)
            {
                numberText.text = "J";
                numberText.color = colorPallet.GetColor(Suit.Black);
            }
            else
            {
                numberText.text = number.ToString();
                numberText.color = colorPallet.GetColor(Suit.White);
            }

            title.text = data.Title;
            description.text = data.Description;
            image.sprite = data.Image;

            var color = colorPallet.GetColor(suit);
            title.color = color;
            description.color = color;
            suitImage.color = color;
            image.color = color;

            _cardData = data;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (_isHovered) return;
            _isHovered = true;
            AnimateHoverEnter();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (!_isHovered) return;
            _isHovered = false;
            AnimateReturnToDefault();
        }

        public void SetOriginalValues()
        {
            _originalSiblingIndex = transform.GetSiblingIndex();
            _originalPosition = transform.localPosition;
            _originalRotation = transform.localRotation.eulerAngles;
        }

        public void AnimateHoverEnter()
        {
            _currentTween?.Kill();

            transform.SetAsLastSibling();
            _currentTween = DOTween.Sequence()
                .Append(transform.DOLocalRotate(Vector3.zero, hoverRotationDuration).SetEase(hoverEaseType))
                .Join(transform.DOScale(_originalScale * hoverScaleFactor, hoverRotationDuration).SetEase(hoverEaseType))
                .OnComplete(() => _currentTween = null);
        }

        public void AnimateReturnToDefault()
        {
            _currentTween?.Kill();

            _currentTween = DOTween.Sequence()
                .Append(transform.DOLocalMove(_originalPosition, hoverRotationDuration).SetEase(hoverEaseType))
                .Join(transform.DOLocalRotate(_originalRotation, hoverRotationDuration).SetEase(hoverEaseType))
                .Join(transform.DOScale(_originalScale, hoverRotationDuration).SetEase(hoverEaseType))
                .OnComplete(() =>
                {
                    transform.SetSiblingIndex(_originalSiblingIndex);
                    _currentTween = null;
                });
        }

        public void AnimateToPosition(Vector3 position, float duration, Ease ease)
        {
            _currentTween?.Kill();
            _currentTween = transform.DOLocalMove(position, duration).SetEase(ease).OnComplete(() => _currentTween = null);
        }

        public void AnimateRotation(Vector3 rotation, float duration, Ease ease)
        {
            _currentTween?.Kill();
            _currentTween = transform.DOLocalRotate(rotation, duration).SetEase(ease).OnComplete(() => _currentTween = null);
        }

        public Tween AnimateToScale(Vector3 scale, float duration, Ease ease)
        {
            _currentTween?.Kill();
            Tween tween = transform.DOScale(scale, duration).SetEase(ease);
            tween.OnComplete(() => _currentTween = null);
            return tween;
        }
    }
}