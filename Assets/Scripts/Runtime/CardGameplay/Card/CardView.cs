﻿using System;
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
        [SerializeField, TabGroup("Draw")] private TextMeshProUGUI _title;
        [SerializeField, TabGroup("Draw")] private TextMeshProUGUI _description;
        [SerializeField, TabGroup("Draw")] private TextMeshProUGUI _rankText;
        [SerializeField, TabGroup("Draw")] private TextMeshProUGUI _energyText;
        [SerializeField, TabGroup("Draw")] private Image _image;
        [SerializeField, TabGroup("Draw")] private Image _suitImage;
        [SerializeField, TabGroup("Draw")] private SuitColorPallet _colorPallet;

        private Transform _discardToLocation, _drawFromLocation;
        [SerializeField] private float _cardMovementDuration;
        [SerializeField] private float _minScale;
        [SerializeField] private Ease _cardMovementEase;

        [SerializeField, TabGroup("Hover Animation")]
        private float _hoverScaleFactor = 1.2f;

        [SerializeField, TabGroup("Hover Animation")]
        private float _hoverRotationDuration = 0.3f;

        [SerializeField, TabGroup("Hover Animation")]
        private float _onOverMoveToY;

        [SerializeField, TabGroup("Hover Animation")]
        private Ease _hoverEaseType = Ease.OutQuad;

        [ShowInInspector, ReadOnly] private CardData _cardData;

        [SerializeField, TabGroup("Outline")] private UIOutline _uiOutline;
        [SerializeField, TabGroup("Outline")] private float _outlineTransitionDuration;

        [SerializeField, TabGroup("Outline")] private float _outlineAlphaMin = 0;
        [SerializeField, TabGroup("Outline")] private float _outlineAlphaMax = 1;


        private Vector3 _originalScale;
        private Vector3 _originalPosition;
        private Vector3 _originalRotation;
        private int _originalSiblingIndex;
        private CardController _controller;

        private Tween _currentTween;
        private bool _isHovered;


        private void Awake()
        {
            _originalScale = transform.localScale;
            SetOriginalValues();
        }

        public CardView Init(Transform drawFrom, Transform discardTo)
        {
            _drawFromLocation = drawFrom;
            _discardToLocation = discardTo;

            return this;
        }

        [Button]
        private CardView Draw(CardData data, int rank, Suit suit = Suit.Default, int potency = 0)
        {
            if (suit == Suit.Default)
            {
                suit = data.Suit;
            }

            if (suit == Suit.White)
            {
                _rankText.text = "J";
                _rankText.color = _colorPallet.GetColor(Suit.Black);
            }
            else
            {
                _rankText.text = rank.ToString();
                _rankText.color = _colorPallet.GetColor(Suit.White);
            }

            _title.text = data.Title;
            _description.text = FormatTextWithPotencyValue(data.Description, potency);
            _energyText.text = data.EnergyCost.ToString();
            _image.sprite = data.Image;

            var color = _colorPallet.GetColor(suit);
            _suitImage.color = color;
            _image.color = color;

            _cardData = data;

            return this;
        }


        public void Draw(CardController controller)
        {
            Draw(controller.Data, controller.Rank, controller.Suit, controller.Potency);
            _controller = controller;
            _controller.IsPlayable.OnValueChanged += isPlayable =>
            {
                if (isPlayable)
                {
                    DOTween.To(() => _uiOutline.color.a,
                        a => _uiOutline.color =
                            new Color(_uiOutline.color.r, _uiOutline.color.g, _uiOutline.color.b, a),
                        _outlineAlphaMax,
                        _outlineTransitionDuration);
                }
                else
                {
                    DOTween.To(() => _uiOutline.color.a,
                        a => _uiOutline.color =
                            new Color(_uiOutline.color.r, _uiOutline.color.g, _uiOutline.color.b, a),
                        _outlineAlphaMin,
                        _outlineTransitionDuration);
                }
            };
        }

        private static string FormatTextWithPotencyValue(string description, int potency)
        {
            var newDescription = description.Replace("$potency", potency.ToString());
            return newDescription;
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

        private void AnimateHoverEnter()
        {
            _currentTween?.Kill();

            transform.SetAsLastSibling();
            _currentTween = DOTween.Sequence()
                .Append(transform.DOLocalRotate(Vector3.zero, _hoverRotationDuration).SetEase(_hoverEaseType))
                .Join(transform.DOLocalMoveY(_onOverMoveToY, _hoverRotationDuration))
                .Join(transform.DOScale(_originalScale * _hoverScaleFactor, _hoverRotationDuration)
                    .SetEase(_hoverEaseType))
                .OnComplete(() => _currentTween = null);
        }

        public void AnimateReturnToDefault()
        {
            _currentTween?.Kill();

            _currentTween = DOTween.Sequence()
                .Append(transform.DOLocalMove(_originalPosition, _hoverRotationDuration).SetEase(_hoverEaseType))
                .Join(transform.DOLocalRotate(_originalRotation, _hoverRotationDuration).SetEase(_hoverEaseType))
                .Join(transform.DOScale(_originalScale, _hoverRotationDuration).SetEase(_hoverEaseType))
                .OnComplete(() =>
                {
                    transform.SetSiblingIndex(_originalSiblingIndex);
                    _currentTween = null;
                });
        }

        public void OnDraw()
        {
            transform.position = _drawFromLocation.position;
            transform.localScale = new(_minScale, _minScale, 1);
            transform.DOScale(1, _cardMovementDuration);
        }

        public void OnDiscard()
        {
            transform.DOMove(_discardToLocation.position, _cardMovementDuration).SetEase(_cardMovementEase);
            transform.DOScale(_minScale, _cardMovementDuration)
                .OnComplete(() =>
                {
                    // Disable the view and then reset for future use
                    _controller.Disable();
                });
        }
    }
}