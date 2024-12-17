using System.Collections.Generic;
using DG.Tweening;
using Runtime.CardGameplay.Tooltip;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Runtime.CardGameplay.Card.View
{
    public class CardView : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField, TabGroup("Draw")] private TextMeshProUGUI _title;
        [SerializeField, TabGroup("Draw")] private TextMeshProUGUI _description;
        [SerializeField, TabGroup("Draw")] private Image _image;

        [SerializeField, TabGroup("Dependencies")]
        private CardGlyphView _glyphView;

        [SerializeField, TabGroup("Dependencies")]
        private CardTooltipSystem _cardTooltipSystem;

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

        [ShowInInspector, ReadOnly] private bool _isHoverEnabled;


        [ShowInInspector, ReadOnly] private Vector3 _originalScale;
        [ShowInInspector, ReadOnly] private Vector3 _originalPosition;
        [ShowInInspector, ReadOnly] private Vector3 _originalRotation;
        [ShowInInspector, ReadOnly] private int _originalSiblingIndex;
        [ShowInInspector, ReadOnly] private CardController _controller;

        private Tween _currentTween;

        private List<TooltipData> _tooltip;
        
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
        private CardView Draw(CardData data, List<CardGlyph> glyphs, int potency = 0)
        {
            DrawGlyphs(glyphs);
            _tooltip = data.ToolTips;

            _title.text = data.Title;
            _description.text = FormatTextWithPotencyValue(data.Description, potency);
            _image.sprite = data.Image;

            _cardData = data;

            return this;
        }

        private void DrawGlyphs(List<CardGlyph> glyphs)
        {
            _glyphView.Draw(glyphs);
        }


        public void Draw(CardController controller)
        {
            Draw(controller.Data, controller.Glyphs, controller.Potency);
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
            if (_isHoverEnabled)
            {
                AnimateHoverEnter();
                _cardTooltipSystem.DrawTooltips(_tooltip);

            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (_isHoverEnabled)
            {
                AnimateReturnToDefault();
                _cardTooltipSystem.HideAllTooltips();
            }
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

        private void AnimateReturnToDefault()
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
            _isHoverEnabled = false;
            transform.position = _drawFromLocation.position;
            transform.localScale = new(_minScale, _minScale, 1);
            transform.DOScale(1, _cardMovementDuration).OnComplete(() => _isHoverEnabled = true);
        }

        public void OnDiscard()
        {
            _isHoverEnabled = false;
            transform.DOMove(_discardToLocation.position, _cardMovementDuration).SetEase(_cardMovementEase);
            transform.DOScale(_minScale, _cardMovementDuration)
                .OnComplete(() =>
                {
                    // Disable the view and then reset for future use
                    _controller.Disable();
                });
        }

        public void OnBurn()
        {
            //TODO: Dissolve Shader
            _controller.Disable();
        }
    }
}