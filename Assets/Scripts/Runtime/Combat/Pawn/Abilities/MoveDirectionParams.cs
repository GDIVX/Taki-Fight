using System;
using Runtime.Combat.Pawn;
using Runtime;

namespace Runtime.Combat.Pawn.Abilities
{
    [Serializable]
    public class MoveDirectionParams : StrategyParams
    {
        public MovementDirection Direction;
    }
}
