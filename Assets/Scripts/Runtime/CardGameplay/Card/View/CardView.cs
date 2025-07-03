using System;
using DamageNumbersPro;
using DG.Tweening;
using Runtime.Combat.Pawn;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Runtime.CardGameplay.Card.View
{
    public class CardView : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        // === UI EVENTS ===
        public event Action OnHoverEnter;
        public event Action OnHoverExit;

// === MAIN UI ===
        [TabGroup("Main UI")] [SerializeField] private TextMeshProUGUI _title;
        [TabGroup("Main UI")] [SerializeField] private TextMeshProUGUI _description;
        [TabGroup("Main UI")] [SerializeField] private Image _image;
        [TabGroup("Main UI")] [SerializeField] private TextMeshProUGUI _costText;
        [TabGroup("Main UI")] [SerializeField] private UIOutline _uiOutline;
        [TabGroup("Main UI")] [SerializeField] private CanvasGroup _canvasGroup;

// === HOVER / ANIMATION ===
        [TabGroup("Hover")] [SerializeField] private float _hoverScaleFactor = 1.2f;

        [TabGroup("Hover")] [SerializeField] private float _hoverDuration = 0.3f;

        [TabGroup("Hover")] [SerializeField] private float _onOverMoveToY;

        [TabGroup("Anim")] [SerializeField] private Ease _MoveEase = Ease.InOutSine;
        [TabGroup("Anim")] [SerializeField] private Ease _ScaleEase = Ease.OutQuad;
        [TabGroup("Anim")] [SerializeField] private Ease _rotateEase = Ease.OutQuad;


// === HIGHLIGHT ===
        [TabGroup("Highlight")] [SerializeField]
        private float _outlineTransitionDuration = 0.2f;

        [TabGroup("Highlight")] [SerializeField]
        private float _outlineAlphaMin = 0f;

        [TabGroup("Highlight")] [SerializeField]
        private float _outlineAlphaMax = 1f;

// === SUMMON STATS ===
        [TabGroup("Summon Stats")] [SerializeField]
        private GameObject _pawnContentContainer;

        [TabGroup("Summon Stats")] [SerializeField]
        private TextMeshProUGUI _healthText;

        [TabGroup("Summon Stats")] [SerializeField]
        private TextMeshProUGUI _attackDamageText;

        [TabGroup("Summon Stats")] [SerializeField]
        private TextMeshProUGUI _multistrikeText;

        [TabGroup("Summon Stats")] [SerializeField]
        private TextMeshProUGUI _speedText;

        [TabGroup("Summon Stats")] [SerializeField]
        private TextMeshProUGUI _sizeText;

        [TabGroup("Summon Stats")] [SerializeField]
        private TextMeshProUGUI _rangeText;

// === FLOATING TEXT ===
        [TabGroup("Floating Text")] [SerializeField]
        private GameObject _floatingTextRoot;

        [TabGroup("Floating Text")] [SerializeField]
        private DamageNumberGUI _floatingTextPrefab;


        private Vector3 _rootScale;
        private Vector3 _rootPosition;
        private Vector3 _rootRotation;
        private int _originalSiblingIndex;

        private RectTransform RectTransform { get; set; }

        private void Awake()
        {
            _rootScale = transform.localScale;
            _rootPosition = transform.localPosition;
            _rootRotation = transform.localRotation.eulerAngles;
            _originalSiblingIndex = transform.GetSiblingIndex();
            RectTransform ??= GetComponent<RectTransform>();
        }


        public void UpdateDescription(string description) => _description.text = description;

        public void SetCost(int cost)
        {
            _costText.text = cost.ToString();
            _costText.transform.parent?.gameObject.SetActive(cost > 0);
        }
        public void ToggleBlockRaycast(bool value)
        {
            _canvasGroup.blocksRaycasts = value;
        }

        public void SetTitle(string title) => _title.text = title;

        public void SetImage(Sprite sprite) => _image.sprite = sprite;

        public void SetHighlight(bool enabled)
        {
            DOTween.To(() => _uiOutline.color.a,
                a => _uiOutline.color = new Color(_uiOutline.color.r, _uiOutline.color.g, _uiOutline.color.b, a),
                enabled ? _outlineAlphaMax : _outlineAlphaMin,
                _outlineTransitionDuration);
        }

        public void ShowPawnStats(PawnData pawn)
        {
            _pawnContentContainer.SetActive(true);
            _healthText.text = pawn.Health.ToString();
            _attackDamageText.text = pawn.Damage.ToString();
            _speedText.text = pawn.Speed.ToString();
            _sizeText.text = pawn.Size.x.ToString();
            _rangeText.text = pawn.AttackRange.ToString();

            if (pawn.Attacks > 1)
            {
                _multistrikeText.gameObject.SetActive(true);
                _multistrikeText.text = pawn.Attacks.ToString();
            }
            else
            {
                _multistrikeText.gameObject.SetActive(false);
            }
        }

        public void HidePawnStats()
        {
            _pawnContentContainer.SetActive(false);
        }


        public void OnPointerEnter(PointerEventData eventData)
        {
            AnimateHoverEnter();
            OnHoverEnter?.Invoke();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            AnimateHoverExit();
            OnHoverExit?.Invoke();
        }

        private void AnimateHoverEnter()
        {
            transform.SetAsLastSibling();
            DOTween.Sequence()
                .Append(transform.DOLocalRotate(Vector3.zero, _hoverDuration).SetEase(_ScaleEase))
                .Join(transform.DOLocalMoveY(_onOverMoveToY, _hoverDuration))
                .Join(transform.DOScale(_rootScale * _hoverScaleFactor, _hoverDuration).SetEase(_ScaleEase));
        }

        private void AnimateHoverExit()
        {
            DOTween.Sequence()
                .Append(transform.DOLocalMove(_rootPosition, _hoverDuration).SetEase(_ScaleEase))
                .Join(transform.DOLocalRotate(_rootRotation, _hoverDuration).SetEase(_ScaleEase))
                .Join(transform.DOScale(_rootScale, _hoverDuration).SetEase(_ScaleEase))
                .OnComplete(() => transform.SetSiblingIndex(_originalSiblingIndex));
        }

        public void ShowMessage(string message)
        {
            if (_floatingTextPrefab == null || _floatingTextRoot == null)
            {
                Debug.LogWarning($"Missing floating text setup on {gameObject.name} for ShowMessage().");
                return;
            }

            _floatingTextPrefab.SpawnGUI(_floatingTextRoot.transform as RectTransform, Vector2.up, message);
        }

        public void OnDraw()
        {
            // Bring to front and show entrance animation
            transform.SetAsLastSibling();
            transform.localScale = Vector3.zero;

            MoveToRoot(0.3f);
        }

        public void OnDiscard()
        {
            DOTween.Sequence()
                .Append(transform.DOScale(Vector3.zero, 0.2f).SetEase(Ease.InBack))
                .OnComplete(() => gameObject.SetActive(false));
        }

        public void OnConsume()
        {
            DOTween.Sequence()
                .Append(transform.DOScale(Vector3.zero, 0.15f).SetEase(Ease.InBack))
                .Join(transform.DOLocalRotate(new Vector3(0, 0, 180), 0.15f).SetEase(Ease.InBack))
                .OnComplete(() => gameObject.SetActive(false));
        }


        /// <summary>
        /// Animate the card’s RectTransform to the given local position & rotation.
        /// </summary>
        public Tween Move(Vector3 targetPosition, Vector3 targetRotation, float duration)
        {
            // Cancel any ongoing tweens on this transform
            DOTween.Kill(transform);

            return DOTween.Sequence()
                .Join(RectTransform.DOLocalMove(targetPosition, duration).SetEase(_MoveEase))
                .Join(RectTransform.DOLocalRotate(targetRotation, duration).SetEase(_rotateEase));
        }

        /// <summary>
        /// Reset only the transform (position, rotation, scale) back to its original values.
        /// Does NOT touch sibling index.
        /// </summary>
        /// <param name="duration"></param>
        public Tween MoveToRoot(float duration)
        {
            DOTween.Kill(transform);
            return DOTween.Sequence()
                .Join(transform.DOScale(_rootScale, duration / 3).SetEase(_ScaleEase))
                .Join(transform.DOLocalRotate(_rootRotation, duration / 3).SetEase(_rotateEase))
                .Join(transform.DOLocalMove(_rootPosition, duration / 3).SetEase(_MoveEase));
        }

        public void SetRoot(Vector3 position, Vector3 rotation, Vector3 scale)
        {
            _rootPosition = position;
            _rootRotation = rotation;
            _rootScale = scale;
        }

        public void SetRoot(Vector3 position, Vector3 rotation)
        {
            SetRoot(position, rotation, _rootScale);
        }
    }
}