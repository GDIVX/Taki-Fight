using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sirenix.OdinInspector;
using Utilities;

namespace Runtime.Combat.Pawn.Targeting
{
    public class PawnTargetingService : Singleton<PawnTargetingService>
    {
        [ShowInInspector, ReadOnly] public bool IsLookingForTarget { get; private set; }
        [ShowInInspector, ReadOnly] public PawnTarget TargetedPawn { get; private set; }
        private TaskCompletionSource<PawnTarget> _targetCompletionSource;
        private PawnTarget _lastTargetedPawn;

        public event Action<PawnTarget> OnTargetFound;

        [Button]
        public async Task<PawnTarget> RequestTargetAsync()
        {
            if (IsLookingForTarget)
            {
                throw new InvalidOperationException("A target selection is already in progress.");
            }

            IsLookingForTarget = true;
            _targetCompletionSource = new TaskCompletionSource<PawnTarget>();

            // Wait until a target is selected or the action is canceled
            PawnTarget target = await _targetCompletionSource.Task;

            IsLookingForTarget = false;
            _lastTargetedPawn = target;
            OnTargetFound?.Invoke(target);
            return target;
        }

        public void SelectTarget(PawnTarget target)
        {
            if (!IsLookingForTarget || _targetCompletionSource == null)
            {
                throw new InvalidOperationException("No target selection is in progress.");
            }

            TargetedPawn = target;
            _targetCompletionSource.SetResult(target);
        }

        public void CancelTargeting()
        {
            if (IsLookingForTarget && _targetCompletionSource != null)
            {
                _targetCompletionSource.SetCanceled();
                IsLookingForTarget = false;
                _targetCompletionSource = null;
            }
        }

        public PawnTarget GetLastTargetedPawn()
        {
            return _lastTargetedPawn;
        }
    }
}