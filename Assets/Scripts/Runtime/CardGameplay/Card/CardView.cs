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
        private bool _lockAnimation = false;

        private void Awake()
        {
            _originalScale = transform.localScale;
        }

        [Button]
        public void Draw(CardData data, int number, Suit suit = Suit.Defualt)
        {
            if (suit == Suit.Defualt)
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
            if (_lockAnimation) return;

            _lockAnimation = true;


            transform.SetAsLastSibling();
            transform.DOLocalRotate(Vector3.zero, hoverRotationDuration).SetEase(hoverEaseType);
            transform.DOScale(_originalScale * hoverScaleFactor, hoverRotationDuration).SetEase(hoverEaseType);

            _lockAnimation = false;
        }

        public void SetOriginalValues()
        {
            _originalSiblingIndex = transform.GetSiblingIndex();
            _originalPosition = transform.localPosition;
            _originalRotation = transform.localRotation.eulerAngles;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            ReturnToDefault();
        }

        public void ReturnToDefault()
        {
            if (_lockAnimation) return;
            _lockAnimation = true;

            transform.DOLocalMove(_originalPosition, hoverRotationDuration).SetEase(hoverEaseType);
            transform.DOLocalRotate(_originalRotation, hoverRotationDuration).SetEase(hoverEaseType);
            transform.DOScale(_originalScale, hoverRotationDuration).SetEase(hoverEaseType);

            transform.SetSiblingIndex(_originalSiblingIndex);
            _lockAnimation = false;
        }
    }
}