using System;
using UnityEngine;
using UnityEngine.EventSystems;
using Sirenix.OdinInspector;

namespace Runtime.Combat.Pawn.Targeting
{
    public class PawnTarget : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField, Required] private PawnController _controller;

        public PawnController Controller => _controller;

        private bool IsValidTarget(out string errorMessage)
        {
            if (!PawnTargetingService.Instance.IsLookingForTarget)
            {
                errorMessage = "Attempted to set target while targeting service is still looking for target";
                return false;
            }

            if (_controller == null)
            {
                errorMessage = $"Pawn Target {name} has no {nameof(PawnController)} assigned.";
                return false;
            }

            errorMessage = $"Attempted to select a dead pawn {name}";
            return !_controller.Health.IsDead();
        }

        private void OnValidate()
        {
            _controller ??= GetComponentInChildren<PawnController>();
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