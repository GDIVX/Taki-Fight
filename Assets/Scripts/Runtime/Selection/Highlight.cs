using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Runtime.Selection
{
    [RequireComponent(typeof(Collider2D))]
    public class Highlight : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private float _blendTime = 0.2f;
        [SerializeField] private Ease _blendEase = Ease.OutQuad;
        [SerializeField] private float _colorBlendTime = 0.2f;
        [SerializeField] private Ease _colorBlendEase = Ease.Linear;

        [SerializeField, ColorUsage(true, true)]
        private Color _lowerPassiveColor, _upperPassiveColor;

        [SerializeField, ColorUsage(true, true)]
        private Color _lowerActiveColor, _upperActiveColor;

        private static readonly int OutlineBlend = Shader.PropertyToID("_OutlineBlend");
        private static readonly int OutlineColorA = Shader.PropertyToID("_OutlineColorA");
        private static readonly int OutlineColorB = Shader.PropertyToID("_OutlineColorB");
        private static readonly int OutlineColorLerp = Shader.PropertyToID("_OutlineColorLerp");

        private Material _instanceMaterial;
        private Tween _outlineTween;
        private Tween _colorTween;
        private bool _isHovered;
        private bool _isActive;

        private void Awake()
        {
            if (_spriteRenderer == null)
            {
                _spriteRenderer = GetComponent<SpriteRenderer>();
            }

            if (_spriteRenderer == null)
            {
                Debug.LogError("SpriteRenderer component is missing.");
                return;
            }

            _instanceMaterial = _spriteRenderer.material;
        }

        private void OnDestroy()
        {
            if (_instanceMaterial != null)
            {
                Destroy(_instanceMaterial);
            }
        }

        private void OnValidate()
        {
            if (_spriteRenderer == null)
            {
                _spriteRenderer = GetComponent<SpriteRenderer>();
            }
        }

        public void Show()
        {
            _isActive = true;
            UpdateHighlight();
        }

        public void Hide()
        {
            _isActive = false;
            _outlineTween?.Kill();
            _colorTween?.Kill();
            ResetMaterial();
        }

        private void UpdateHighlight()
        {
            if (!_isActive) return;

            Color colorA = _isHovered ? _lowerActiveColor : _lowerPassiveColor;
            Color colorB = _isHovered ? _upperActiveColor : _upperPassiveColor;

            _instanceMaterial.SetColor(OutlineColorA, colorA);
            _instanceMaterial.SetColor(OutlineColorB, colorB);

            _outlineTween?.Kill();
            _colorTween?.Kill();

            _outlineTween = DOTween.To(
                    (x) => _instanceMaterial.SetFloat(OutlineBlend, x),
                    0, 1, _blendTime)
                .SetEase(_blendEase)
                .OnComplete(() =>
                {
                    _colorTween = DOTween.To(
                            (x) => _instanceMaterial.SetFloat(OutlineColorLerp, x),
                            0, 1, _colorBlendTime)
                        .SetEase(_colorBlendEase);
                });
        }

        private void ResetMaterial()
        {
            DOTween.To(
                    (x) => _instanceMaterial.SetFloat(OutlineBlend, x),
                    _instanceMaterial.GetFloat(OutlineBlend), 0, _blendTime)
                .SetEase(_blendEase);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _isHovered = true;
            UpdateHighlight();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _isHovered = false;
            UpdateHighlight();
        }
    }
}