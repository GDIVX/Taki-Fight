using System.Threading.Tasks;
using Runtime.Combat.Pawn.Targeting;
using UnityEngine;

namespace Runtime.CardGameplay.Card.CardBehaviour
{
    [CreateAssetMenu(fileName = "TargetedCardSelect", menuName = "CardGameplay/SelectStrategies/TargetedCardSelect")]
    public class TargetedCardSelect : CardSelectStrategy
    {
        public override async Task<bool> SelectAsync(CardController card)
        {
            // Start looking for a pawn target
            PawnTarget target = await PawnTargetingService.Instance.RequestTargetAsync();

            // If a target is selected, return true, otherwise return false
            return target != null;
        }
    }
}