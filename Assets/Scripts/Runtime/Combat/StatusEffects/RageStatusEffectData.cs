using System;
using Runtime.Combat.Pawn;
using UnityEngine;
using Utilities;

namespace Runtime.Combat.StatusEffects
{
    [CreateAssetMenu(fileName = "RageStFX", menuName = "StatusEffect/Rage", order = 0)]
    public class RageStatusEffectData : StatusEffectData
    {
        public override IStatusEffect CreateStatusEffect(int stacks)
        {
            return new RageStatusEffect()
            {
                Stack = new TrackedProperty<int>()
                {
                    Value = stacks
                }
            };
        }
    }

    public class RageStatusEffect : IStatusEffect
    {
        public TrackedProperty<int> Stack { get; set; }

        public void Apply(PawnController pawn)
        {
            pawn.AttackModifier.Value = Stack.Value;
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