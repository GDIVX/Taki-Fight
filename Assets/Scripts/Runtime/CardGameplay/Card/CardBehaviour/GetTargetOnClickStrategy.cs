using Runtime.Combat.Pawn;
using Runtime.Combat.Pawn.Targeting;
using UnityEngine;

namespace Runtime.CardGameplay.Card.CardBehaviour
{
    [CreateAssetMenu(fileName = "Get Target On Click Strategy", menuName = "Card/Strategy/Targeting/On Click", order = 0)]
    public class GetTargetOnClickStrategy : TargetingStrategy
    {
        public override PawnController GetTarget()
        {
            return PawnTargetingService.Instance.TargetedPawn.Controller;
        }
    }
}