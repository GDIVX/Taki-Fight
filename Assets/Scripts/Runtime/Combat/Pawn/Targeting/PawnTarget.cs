using UnityEngine;
using UnityEngine.EventSystems;
using Sirenix.OdinInspector;

namespace Runtime.Combat.Pawn.Targeting
{
    public class PawnTarget : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField, Required] private PawnController controller;

        public PawnController Controller => controller;

        private bool IsValidTarget(out string errorMessage)
        {
            if (!PawnTargetingService.Instance.IsLookingForTarget)
            {
                errorMessage = "Attempted to set target while targeting service is still looking for target";
                return false;
            }

            if (controller == null)
            {
                errorMessage = $"Pawn Target {name} has no {nameof(PawnController)} assigned.";
                return false;
            }

            errorMessage = $"Attempted to select a dead pawn {name}";
            return !controller.Health.IsDead();
        }

        [Button]
        public void SetAsTarget()
        {
            if (!IsValidTarget(out string errorMessage))
            {
                Debug.LogWarning(errorMessage);
                return;
            }

            PawnTargetingService.Instance.SelectTarget(this);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            SetAsTarget();
        }
    }
}