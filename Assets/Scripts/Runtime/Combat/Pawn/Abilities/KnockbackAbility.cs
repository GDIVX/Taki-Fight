using System;
using Runtime.Combat.Tilemap;
using UnityEngine;
using Utilities;

namespace Runtime.Combat.Pawn.Abilities
{
    [CreateAssetMenu(fileName = "Knockback Ability", menuName = "Pawns/Abilities/Combat/Knockback", order = 0)]
    public class KnockbackAbility : PawnHitPlayStrategy
    {
        private const int DamagePerTile = 3;

        public override void Play(PawnController pawn, PawnController target, ref int damage, Action<bool> onComplete)
        {
            if (target == null)
            {
                Debug.LogError("KnockbackAbility called with null target.");
                onComplete?.Invoke(false);
                return;
            }

            var tilemap = ServiceLocator.Get<TilemapController>();
            var direction = pawn.Owner == PawnOwner.Player ? Vector2Int.right : Vector2Int.left;
            var moved = 0;
            var anchor = target.TilemapHelper.AnchorTile.Position;

            for (int i = 0; i < Potency; i++)
            {
                var next = anchor + direction;
                var tile = tilemap.GetTile(next);
                if (tile != null && !tile.IsOccupied)
                {
                    anchor = next;
                    moved++;
                }
                else
                {
                    break;
                }
            }

            if (moved > 0)
            {
                var tile = tilemap.GetTile(anchor);
                target.MoveToPosition(tile, null);
            }

            if (moved < Potency)
            {
                int missing = Potency - moved;
                target.Combat.ReceiveAttack(DamagePerTile * missing);
            }

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