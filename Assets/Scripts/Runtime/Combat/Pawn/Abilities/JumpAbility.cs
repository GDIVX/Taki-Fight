using System;
using Runtime.Combat.Tilemap;
using UnityEngine;
using Utilities;

namespace Runtime.Combat.Pawn.Abilities
{
    [CreateAssetMenu(fileName = "Jump Ability", menuName = "Pawns/Abilities/Movement/Jump", order = 0)]
    public class JumpAbility : PawnMovePlayStrategy
    {
        public override void ModifyMove(PawnController pawn, ref Tile nextTile)
        {
            var tilemap = ServiceLocator.Get<TilemapController>();
            if (tilemap == null || nextTile == null) return;

            if (!nextTile.IsOccupied) return;

            var direction = pawn.Owner == PawnOwner.Player ? Vector2Int.right : Vector2Int.left;
            var jumpPosition = nextTile.Position + direction;
            var candidate = tilemap.GetTile(jumpPosition);
            if (candidate != null)
            {
                nextTile = candidate;
            }
        }

        public override string GetDescription()
        {
            return "Jump over one tile";
        }

        public override void Play(PawnController pawn, Action<bool> onComplete)
        {
        }
    }
}
