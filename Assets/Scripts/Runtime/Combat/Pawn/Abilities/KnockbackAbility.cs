using System;
using UnityEngine;

namespace Runtime.Combat.Pawn.Abilities
{
    [CreateAssetMenu(fileName = "Knockback Ability", menuName = "Pawns/Abilities/Combat/Knockback", order = 0)]
    public class KnockbackAbility : PawnHitPlayStrategy
    {
        private const int DamagePerTile = 3;

        public override void Play(PawnController pawn, PawnController target, ref int damage, Action<bool> onComplete)
        {
            var direction = pawn.Owner == PawnOwner.Player ? Vector2Int.up : Vector2Int.left;
            PawnHelper.Knockback(target, Potency, DamagePerTile, direction, onComplete);

            onComplete?.Invoke(true);
        }

        public override string GetDescription()
        {
            return $"Knockback {Potency}";
        }

        public override void Play(PawnController pawn, Action<bool> onComplete)
        {
        }
    }
}