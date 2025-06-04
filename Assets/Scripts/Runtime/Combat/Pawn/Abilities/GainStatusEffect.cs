using System;
using Runtime.Combat.StatusEffects;
using UnityEngine;

namespace Runtime.Combat.Pawn.Abilities
{
    [CreateAssetMenu(fileName = "Gain Status Effect", menuName = "Pawns/Abilities/Gain Status Effect", order = 0)]
    public class GainStatusEffect : PawnPlayStrategy
    {
        [SerializeField] private StatusEffectData _statusEffectData;

        public override void Play(PawnController pawn, Action<bool> onComplete)
        {
            pawn.ApplyStatusEffect(_statusEffectData, Potency);
        }
    }
}