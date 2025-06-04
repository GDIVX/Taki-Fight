using System;
using Runtime.Combat.Pawn;
using Runtime;

namespace Runtime.Combat.Pawn.Abilities
{
    [Serializable]
    public class MoveAbilityParams : StrategyParams
    {
        public MovementDirection Direction;
    }
}
