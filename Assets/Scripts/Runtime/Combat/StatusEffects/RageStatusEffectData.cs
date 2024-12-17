using System;
using Runtime.Combat.Pawn;
using UnityEngine;

namespace Runtime.Combat.StatusEffects
{
    [CreateAssetMenu(fileName = "RageStFX", menuName = "StatusEffect/Rage", order = 0)]
    public class RageStatusEffectData : StatusEffectData
    {
        public override IStatusEffect CreateStatusEffect(int stacks)
        {
            return new RageStatusEffect()
            {
                Stack = stacks
            };
        }
    }

    public class RageStatusEffect : IStatusEffect
    {
        public event Action<IStatusEffect> OnApplied;
        public event Action<IStatusEffect> OnRemoved;
        public int Stack { get; set; }

        public void Apply(PawnController pawn)
        {
            pawn.AttackModifier.Value = Stack;
        }

        public void OnAdded(PawnController pawn)
        {
            Apply(pawn);
        }

        public void Remove(PawnController pawn)
        {
            pawn.AttackModifier.Value = 0;
        }
    }
}