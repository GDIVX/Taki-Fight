using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Runtime.Combat.Pawn.Targeting
{
    [RequireComponent(typeof(Collider2D))]
    public class PawnTargetView : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private PawnTarget _pawnTarget;
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private float _blendTime;
        [SerializeField] private Ease _blendEase;
        [SerializeField] private float _colorBlendTime;
        [SerializeField] private Ease _colorbBlendEase;

        [SerializeField, ColorUsage(true, true)]
        private Color _outlineColorForHeroA, _outLineColorForEnemyA;

        [SerializeField, ColorUsage(true, true)]
        private Color _outlineColorForHeroB, _outLineColorForEnemyB;

        private static readonly int OutlineBlend = Shader.PropertyToID("_OutlineBlend");
        private static readonly int OutlineColorA = Shader.PropertyToID("_OutlineColorA");
        private static readonly int OutlineColorB = Shader.PropertyToID("_OutlineColorB");
        private static readonly int OutlineColorLerp = Shader.PropertyToID("_OutlineColorLerp");


        private void Awake()
        {
            _pawnTarget ??= GetComponent<PawnTarget>();
            if (_spriteRenderer == null)
            {
                _spriteRenderer = GetComponent<SpriteRenderer>();
            }

            if (_spriteRenderer != null) return;
            Debug.LogError("SpriteRenderer component is missing.");
            return;
        }

        private void OnValidate()
        {
            if (_spriteRenderer == null)
            {
                _spriteRenderer = GetComponent<SpriteRenderer>();
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!_pawnTarget.IsValidTarget())
            {
                ResetMaterial();
                return;
            }

            if (_spriteRenderer == null)
            {
                Debug.LogError($"{nameof(SpriteRenderer)} was not assigned");
                return;
            }

            var colorA = _pawnTarget.PawnTargetType == PawnTargetType.Hero
                ? _outlineColorForHeroA
                : _outLineColorForEnemyA;

            var colorB = _pawnTarget.PawnTargetType == PawnTargetType.Hero
                ? _outlineColorForHeroB
                : _outLineColorForEnemyB;

            _spriteRenderer.material.SetColor(OutlineColorA, colorA);
            _spriteRenderer.material.SetColor(OutlineColorB, colorB);
            DOTween.To((x) => _spriteRenderer.material.SetFloat(OutlineBlend, x), 0, 1, _blendTime)
                .SetEase(_blendEase).onComplete += () =>
            {
                DOTween.To(
                        (x) => _spriteRenderer.material.SetFloat(OutlineColorLerp, x), 0, 1, _colorBlendTime)
                    .SetEase(_colorbBlendEase);
            };
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (_spriteRenderer == null) return;

            ResetMaterial();
        }

        private void ResetMaterial()
        {
            DOTween.To((x) => _spriteRenderer.material.SetFloat(OutlineBlend, x),
                    _spriteRenderer.material.GetFloat(OutlineBlend), 0, _blendTime)
                .SetEase(_blendEase);
        }
    }
}