using UnityEngine;
using UnityEngine.EventSystems;
using Sirenix.OdinInspector;

namespace Runtime.Combat.Pawn.Targeting
{
    public class PawnTarget : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private PawnController controller;

        public PawnController Controller => controller;

        private bool IsValidTarget()
        {
            return PawnTargetingService.Instance.IsLookingForTarget && controller != null &&
                   !controller.Health.IsDead();
        }

        [Button]
        public void SetAsTarget()
        {
            if (!IsValidTarget())
            {
                Debug.LogWarning("Attempted to set an invalid target.");
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