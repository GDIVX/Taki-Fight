using System;
using Runtime.Combat.StatusEffects;
using UnityEngine;

namespace Runtime.Combat.Pawn.Abilities
{
    [CreateAssetMenu(fileName = "Gain Status Effect", menuName = "Pawns/Abilities/Gain Status Effect", order = 0)]
    public class GainStatusEffect : PawnPlayStrategy
    {
        private GainStatusEffectParams _params;

        public override void Play(PawnController pawn, Action<bool> onComplete)
        {
            pawn.ApplyStatusEffect(_params.StatusEffect, Potency);
        }

        public override void Initialize(PawnStrategyData data)
        {
            _params = data.Parameters as GainStatusEffectParams;
            base.Initialize(data);
        }
    }
}