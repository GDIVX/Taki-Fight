using DG.Tweening;
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
        [SerializeField, TabGroup("Draw")] private TextMeshProUGUI _costText;

        [SerializeField, TabGroup("Dissolve")] private Image _mask;
        [SerializeField, TabGroup("Dissolve")] private CanvasGroup _canvasGroup;
        [SerializeField, TabGroup("Dissolve")] private float _dissolveTime;
        [SerializeField, TabGroup("Dissolve")] private Ease _dissolveEase;


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

        [SerializeField, TabGroup("Outline")] private UIOutline _uiOutline;
        [SerializeField, TabGroup("Outline")] private float _outlineTransitionDuration;

        [SerializeField, TabGroup("Outline")] private float _outlineAlphaMin;
        [SerializeField, TabGroup("Outline")] private float _outlineAlphaMax = 1;

        [ShowInInspector] [ReadOnly] private CardData _cardData;
        [ShowInInspector] [ReadOnly] private CardController _controller;

        private Tween _currentTween;

        private Transform _discardToLocation, _drawFromLocation;

        [ShowInInspector, ReadOnly] private bool _isHoverEnabled;

        public bool IsAnimating => _currentTween != null && _currentTween.IsActive() && _currentTween.IsPlaying();

        public void SetHoverEnabled(bool value)
        {
            _isHoverEnabled = value;
        }

        [ShowInInspector, ReadOnly] private Vector3 _originalPosition;
        [ShowInInspector, ReadOnly] private Vector3 _originalRotation;


        [ShowInInspector] [ReadOnly] private Vector3 _originalScale;
        [ShowInInspector, ReadOnly] private int _originalSiblingIndex;


        private void Awake()
        {
            _originalScale = transform.localScale;
            SetOriginalValues();
        }


        public void OnPointerEnter(PointerEventData eventData)
        {
            if (_isHoverEnabled) AnimateHoverEnter();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (_isHoverEnabled) AnimateReturnToDefault();
        }

        public CardView Init(Transform drawFrom, Transform discardTo)
        {
            _drawFromLocation = drawFrom;
            _discardToLocation = discardTo;
            _canvasGroup.interactable = true;
            _canvasGroup.blocksRaycasts = true;

            return this;
        }

        [Button]
        private CardView Draw(CardData data)
        {
            _title.text = data.Title;
            _image.sprite = data.Image;

            var cost = data.Cost;
            _costText.text = cost.ToString();

            _costText.transform.parent.gameObject.SetActive(cost > 0);

            _mask.fillAmount = 1;
            _canvasGroup.alpha = 1;

            _cardData = data;

            return this;
        }

        [Button]
        public void SetOutlineColor(Color color)
        {
            _uiOutline.color = color;
        }


        public void Draw(CardController controller)
        {
            Draw(controller.Data);
            _controller = controller;
            UpdateDescription();
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

        public void UpdateDescription()
        {
            var builder = new DescriptionBuilder();
            _description.text = builder.Build(_cardData);
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

        [Button]
        public void OnBurn()
        {
            _canvasGroup.interactable = false;
            _canvasGroup.blocksRaycasts = false;

            AnimateHoverEnter();
            _mask.DOFillAmount(0, _dissolveTime).SetEase(_dissolveEase);
            _canvasGroup.DOFade(0, _dissolveTime).SetEase(_dissolveEase).onComplete += () =>
            {
                _controller.Disable();
                transform.localPosition = _originalPosition;
                transform.localRotation = Quaternion.Euler(_originalRotation);
                transform.localScale = _originalScale;
            };
        }
    }
}