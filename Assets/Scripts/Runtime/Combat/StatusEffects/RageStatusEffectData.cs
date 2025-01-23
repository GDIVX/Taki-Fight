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

        public override Type GetStatusEffectType()
        {
            return typeof(RageStatusEffect);
        }
    }

    public class RageStatusEffect : IStatusEffect
    {
        public TrackedProperty<int> Stack { get; set; }

        private int _previousStack;

        public void OnAdded(PawnController pawn)
        {
            pawn.AttackModifier.Value += Stack.Value;
            _previousStack = Stack.Value;
        }

        public void OnTurnStart(PawnController pawn)
        {
            // Remove the old bonus and apply the new one
            pawn.AttackModifier.Value -= _previousStack;
            pawn.AttackModifier.Value += Stack.Value;
            _previousStack = Stack.Value;
        }

        public void Remove(PawnController pawn)
        {
            pawn.AttackModifier.Value -= Stack.Value;
        }
    }
}