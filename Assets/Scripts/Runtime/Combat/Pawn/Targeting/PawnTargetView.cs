using UnityEngine;
using UnityEngine.EventSystems;

namespace Runtime.Combat.Pawn.Targeting
{
    [RequireComponent(typeof(Collider2D))]
    public class PawnTargetView : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private PawnTarget _pawnTarget;
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private Material _highlightMaterial;
        private Material _originalMaterial;

        public bool IsValidTarget { get; set; }

        private void Awake()
        {
            _pawnTarget ??= GetComponent<PawnTarget>();
            if (_spriteRenderer == null)
            {
                _spriteRenderer = GetComponent<SpriteRenderer>();
            }

            if (_spriteRenderer == null)
            {
                Debug.LogError("SpriteRenderer component is missing.");
                return;
            }

            _originalMaterial = _spriteRenderer.material;
        }

        private void OnValidate()
        {
            if (_spriteRenderer == null)
            {
                _spriteRenderer = GetComponent<SpriteRenderer>();
            }

            if (_highlightMaterial == null)
            {
                Debug.LogWarning("Highlight material is not assigned.");
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

            if (_highlightMaterial == null)
            {
                Debug.LogError($"Highlight material was not assigned");
                return;
            }

            _spriteRenderer.material = _highlightMaterial;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (_spriteRenderer == null) return;

            ResetMaterial();
        }

        private void ResetMaterial()
        {
            if (_spriteRenderer.material != _originalMaterial)
                _spriteRenderer.material = _originalMaterial;
        }
    }
}