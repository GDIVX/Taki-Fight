using System.Threading.Tasks;
using Runtime.Combat.Pawn.Targeting;
using UnityEngine;

namespace Runtime.CardGameplay.Card.CardBehaviour
{
    [CreateAssetMenu(fileName = "TargetedCardSelect", menuName = "Card/Strategy/Select/TargetedCardSelect")]
    public class TargetedCardSelect : CardSelectStrategy
    {
        public override async Task<bool> SelectAsync(CardController card)
        {
            try
            {
                // Start looking for a pawn target
                PawnTarget target = await PawnTargetingService.Instance.RequestTargetAsync();

                // If a target is selected, return true, otherwise return false
                return target != null;
            }
            catch (TaskCanceledException)
            {
                // Handle cancellation gracefully
                return false;
            }
        }
    }
}