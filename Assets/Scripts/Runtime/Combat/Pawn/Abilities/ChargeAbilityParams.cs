using System;

namespace Runtime.Combat.Pawn.Abilities
{
    [Serializable]
    public class ChargeAbilityParams : StrategyParams
    {
        public int MovementDistance;
        public int KnockbackStrength;
    }
}
