using System;
using DG.Tweening;
using Runtime.Combat.Pawn;
using Runtime.Combat.Pawn.Targeting;
using UnityEngine;

namespace Runtime.CardGameplay.Card.CardBehaviour.Feedback
{
    [CreateAssetMenu(fileName = "Move To Target Feedback", 
        menuName = "Card/Strategy/Feedback/Move To Target", order = 0)]
    public class MoveToTargetFeedback : FeedbackStrategy
    {
        [SerializeField] private float _animationTimeForward = 0.5f;  // Time for caller to move forward
        [SerializeField] private float _animationTimeBackward = 0.5f; // Time for caller to move back
        [SerializeField] private Vector3 _knockbackVector = new Vector3(0, 0, -1); // Knockback direction and distance
        [SerializeField] private float _knockbackDuration = 0.3f;
        [SerializeField] private Vector3 _offset;
        [SerializeField] private Ease _easeForward = Ease.OutQuad;
        [SerializeField] private Ease _easeBackward = Ease.OutQuad;
        [SerializeField] private Ease _knockbackEase = Ease.OutBack;

        public override void Animate(PawnController caller, Action onComplete)
        {
            // Guard clauses
            if (!caller) 
            {
                Debug.LogWarning("Caller is null. Cannot animate.");
                onComplete?.Invoke();
                return;
            }

            var targetController = PawnTargetingService.Instance?.TargetedPawn?.Controller;
            if (!targetController) 
            {
                Debug.LogWarning("No valid target to move toward.");
                onComplete?.Invoke();
                return;
            }

            // Initial positions
            var callerTransform = caller.transform;
            var startPosition = callerTransform.position;
            var targetPosition = targetController.transform.position + _offset;

            // Create a Sequence for better control
            Sequence sequence = DOTween.Sequence();

            // 1. Caller moves forward to the target
            sequence.Append(
                callerTransform.DOMove(targetPosition, _animationTimeForward)
                    .SetEase(_easeForward)
            );

            // 2. Knockback the target at the moment of impact
            sequence.AppendCallback(() =>
            {
                // Calculate knockback target position using the knockback vector
                var knockbackPosition = targetController.transform.position + _knockbackVector;

                // Animate the target's knockback with a yoyo effect
                targetController.transform
                    .DOMove(knockbackPosition, _knockbackDuration)
                    .SetEase(_knockbackEase)
                    .SetLoops(2, LoopType.Yoyo); // Move out and back
            });

            // 3. Caller moves back to its original position
            sequence.Append(
                callerTransform.DOMove(startPosition, _animationTimeBackward)
                    .SetEase(_easeBackward)
            );

            // 4. Invoke the completion callback when the sequence finishes
            sequence.OnComplete(() => onComplete?.Invoke());
        }
    }
}
