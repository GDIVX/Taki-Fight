using Runtime.Combat.Pawn;
using Runtime.Combat.Pawn.Targeting;
using UnityEngine;

namespace Runtime.CardGameplay.Card.CardBehaviour
{
    [CreateAssetMenu(fileName = "Target Player Character", menuName = "Card/Strategy/Targeting/Player", order = 0)]
    public class TargetPlayerCharacterStrategy : TargetingStrategy
    {
        public override PawnController GetTarget()
        {
            return GameManager.Instance.Hero;
        }
    }
}