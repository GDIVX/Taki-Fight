using System;
using Runtime.Combat.StatusEffects;

namespace Runtime.Combat.Pawn.Abilities
{
    [Serializable]
    public class StatusEffectParams : StrategyParams
    {
        public StatusEffectData StatusEffect;
    }
}