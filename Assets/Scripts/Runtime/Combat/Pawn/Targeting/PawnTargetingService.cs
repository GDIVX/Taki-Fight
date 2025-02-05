using System;
using UnityEngine;
using Sirenix.OdinInspector;
using Utilities;

namespace Runtime.Combat.Pawn.Targeting
{
    public class PawnTargetingService : Singleton<PawnTargetingService>
    {
        [ShowInInspector, ReadOnly] public bool IsLookingForTarget { get; private set; }
        [ShowInInspector, ReadOnly] public PawnTarget TargetedPawn { get; private set; }
        public PawnTargetType TargetTypeLookingFor { get; private set; }

        private PawnTarget _lastTargetedPawn;

        public event Action<PawnTarget> OnTargetFound;
        public event Action OnLookingForTarget;
        public event Action OnTargetSelectionCanceled;

        [Button]
        public void RequestTarget(PawnTargetType targetType)
        {
            if (IsLookingForTarget)
            {
                throw new InvalidOperationException("A target selection is already in progress.");
            }

            IsLookingForTarget = true;
            TargetTypeLookingFor = targetType;
            GameManager.Instance.BannerViewManager.WriteMessage(0, "Select Target", Color.yellow);
            OnLookingForTarget?.Invoke();
        }

        public void SelectTarget(PawnTarget target)
        {
            if (!IsLookingForTarget)
            {
                Debug.LogWarning("Attempted to set target while targeting service is not looking for a target.");
                return;
            }

            TargetedPawn = target;
            _lastTargetedPawn = target;
            IsLookingForTarget = false;
            GameManager.Instance.BannerViewManager.Clear();
            OnTargetFound?.Invoke(target);
        }

        public void CancelTargeting()
        {
            if (IsLookingForTarget)
            {
                IsLookingForTarget = false;
                GameManager.Instance.BannerViewManager.Clear();
                OnTargetSelectionCanceled?.Invoke();
            }
        }

        public PawnTarget GetLastTargetedPawn()
        {
            return _lastTargetedPawn;
        }
    }
}