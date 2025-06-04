using System;
using Runtime.Combat.StatusEffects;
using Runtime;

namespace Runtime.Combat.Pawn.Abilities
{
    [Serializable]
    public class GainStatusEffectParams : StrategyParams
    {
        public StatusEffectData StatusEffect;
    }
}
