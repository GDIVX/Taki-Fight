using UnityEngine;
using UnityEngine.EventSystems;

namespace Runtime.Combat.Pawn.Targeting
{
    [RequireComponent(typeof(Collider2D))]
    public class PawnTargetView : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private Material highlightMaterial;
        private Material _originalMaterial;

        private void Awake()
        {
            if (spriteRenderer == null)
            {
                spriteRenderer = GetComponent<SpriteRenderer>();
            }

            if (spriteRenderer == null)
            {
                Debug.LogError("SpriteRenderer component is missing.");
                return;
            }

            _originalMaterial = spriteRenderer.material;
        }

        private void OnValidate()
        {
            if (spriteRenderer == null)
            {
                spriteRenderer = GetComponent<SpriteRenderer>();
            }

            if (highlightMaterial == null)
            {
                Debug.LogWarning("Highlight material is not assigned.");
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!PawnTargetingService.Instance.IsLookingForTarget) return;
            if (spriteRenderer == null)
            {
                Debug.LogError($"{nameof(SpriteRenderer)} was not assigned");
                return;
            }

            if (highlightMaterial == null)
            {
                Debug.LogError($"Highlight material was not assigned");
                return;
            }

            spriteRenderer.material = highlightMaterial;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (!PawnTargetingService.Instance.IsLookingForTarget) return;
            if (spriteRenderer == null) return;

            spriteRenderer.material = _originalMaterial;
        }
    }
}