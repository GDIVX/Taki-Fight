using System;
using Runtime.Combat.Pawn.Targeting;
using UnityEngine;

namespace Runtime.CardGameplay.Card.CardBehaviour
{
    [CreateAssetMenu(fileName = "TargetedCardSelect", menuName = "Card/Strategy/Select/TargetedCardSelect")]
    public class TargetedCardSelect : CardSelectStrategy
    {
        [SerializeField] private PawnTargetType TargetType;

        public override void Select(CardController card, Action<bool> onSelectionComplete)
        {
            // Start looking for a pawn target
            PawnTargetingService.Instance.RequestTarget(TargetType);

            // Subscribe to events
            PawnTargetingService.Instance.OnTargetFound += OnTargetFound;
            PawnTargetingService.Instance.OnTargetSelectionCanceled += OnTargetSelectionCanceled;

            void OnTargetFound(PawnTarget target)
            {
                // Clean up event subscriptions
                PawnTargetingService.Instance.OnTargetFound -= OnTargetFound;
                PawnTargetingService.Instance.OnTargetSelectionCanceled -= OnTargetSelectionCanceled;

                // Notify completion
                onSelectionComplete(true);
            }

            void OnTargetSelectionCanceled()
            {
                // Clean up event subscriptions
                PawnTargetingService.Instance.OnTargetFound -= OnTargetFound;
                PawnTargetingService.Instance.OnTargetSelectionCanceled -= OnTargetSelectionCanceled;

                // Notify completion
                onSelectionComplete(false);
            }
        }
    }
}