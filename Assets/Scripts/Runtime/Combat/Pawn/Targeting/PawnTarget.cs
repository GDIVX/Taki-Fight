using UnityEngine;
using UnityEngine.EventSystems;
using Sirenix.OdinInspector;

namespace Runtime.Combat.Pawn.Targeting
{
    public class PawnTarget : MonoBehaviour, IPointerClickHandler
    {
        public PawnTargetType PawnTargetType { get; private set; }
        public PawnController Controller { get; private set; }

        public void Init(PawnController controller, PawnTargetType targetType)
        {
            Controller = controller;
            PawnTargetType = targetType;
        }

        public bool IsValidTarget()
        {
            if (!PawnTargetingService.Instance.IsLookingForTarget)
            {
                return false;
            }

            if (PawnTargetingService.Instance.TargetTypeLookingFor != PawnTargetType)
            {
                return false;
            }

            if (Controller == null)
            {
                return false;
            }

            return !Controller.Health.IsDead();
        }

        private void OnValidate()
        {
            Controller ??= GetComponentInChildren<PawnController>();
        }

        [Button]
        public void SetAsTarget()
        {
            if (!IsValidTarget())
            {
                return;
            }

            PawnTargetingService.Instance.SelectTarget(this);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            SetAsTarget();
        }
    }

    public enum PawnTargetType
    {
        Hero,
        Enemy
    }
}