using UnityEngine;
using UnityEngine.EventSystems;

namespace Runtime.Combat.Pawn.Targeting
{
    public class PawnTarget : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private PawnController controller;

        public PawnController Controller => controller;
        private bool IsValidTarget() => PawnTargetingService.Instance.IsLookingForTarget && !controller.Health.IsDead();

        public void SetAsTarget()
        {
            if (!IsValidTarget()) return;
            PawnTargetingService.Instance.SelectTarget(this);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (!IsValidTarget()) return;
            SetAsTarget();
        }
    }
}