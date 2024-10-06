using UnityEngine;
using UnityEngine.EventSystems;

namespace Runtime.Combat.Pawn.Targeting
{
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
            _originalMaterial = spriteRenderer.material;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (PawnTargetingService.Instance.IsLookingForTarget)
            {
                spriteRenderer.material = highlightMaterial;
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (PawnTargetingService.Instance.IsLookingForTarget)
            {
                spriteRenderer.material = _originalMaterial;
            }
        }
    }
}