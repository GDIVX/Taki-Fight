using System;
using Runtime.Combat.Pawn;
using UnityEngine;
using Utilities;

namespace Runtime.Combat.StatusEffects
{
    [CreateAssetMenu(fileName = "PowerStFx", menuName = "StatusEffect/Buff/Power")]
    public class DamageGainStatusEffect : StatusEffectStrategy
    {
        [SerializeField] private int _damageScalar;
        [SerializeField] private int _decayRate;

        public override Observable<int> Stack { get; set; } = new();

        private int DamageGain => _damageScalar * Stack.Value;

        public override void OnAdded(PawnController pawn)
        {
            ApplyModifier(pawn);
        }

        public override void OnTurnStart(PawnController pawn)
        {
            if (_decayRate > 0)
                Stack.Value = Mathf.Max(0, Stack.Value - _decayRate);

            ApplyModifier(pawn);
        }

        public override void Remove(PawnController pawn)
        {
            pawn.Combat.Damage.RemoveModifier(this);
        }

        public override object Clone()
        {
            var clone = CreateInstance<DamageGainStatusEffect>();
            clone.Stack = Stack;
            clone._decayRate = _decayRate;
            clone._damageScalar = _damageScalar;
            return clone;
        }

        private void ApplyModifier(PawnController pawn)
        {
            pawn.Combat.Damage.SetModifier(this, DamageGain);
        }
    }
}