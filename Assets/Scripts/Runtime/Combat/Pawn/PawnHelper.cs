using System;
using System.Collections.Generic;
using System.Linq;
using Runtime.Combat.Tilemap;
using UnityEngine;
using Utilities;

namespace Runtime.Combat.Pawn
{
    /// <summary>
    ///     A collection of helper methods to help interact with uncommon mechanics for pawns
    /// </summary>
    public static class PawnHelper
    {
        /// <summary>
        ///     Push a pawn in a direction given magnitude. If it collides with something, it recive damage and so whatever it had
        ///     collided with.
        /// </summary>
        /// <param name="pawn"></param>
        /// <param name="magnitude"></param>
        /// <param name="damagePerTile"></param>
        /// <param name="direction"></param>
        /// <param name="onComplete"></param>
        public static void Knockback(PawnController pawn, int magnitude, int damagePerTile, Vector2Int direction,
            Action<bool> onComplete = null)
        {
            var force = direction * magnitude;
            Knockback(pawn, force, damagePerTile, onComplete);
        }

        public static void Knockback(PawnController pawn, Vector2Int force, int damagePerTile,
            Action<bool> onComplete = null)
        {
            if (pawn == null)
            {
                Debug.LogError("PawnHelper.Knockback called with null pawn.");
                onComplete?.Invoke(false);
                return;
            }

            var tilemap = ServiceLocator.Get<TilemapController>();
            var direction = NormalizeTaxicab(force);
            var magnitude = Mathf.CeilToInt(force.magnitude);
            var moved = 0;
            var currTile = pawn.TilemapHelper.AnchorTile.Position;

            for (var i = 0; i < magnitude; i++)
            {
                var nextPosition = currTile + direction;
                var projectedFootprint = pawn.TilemapHelper.GenerateFootprintUnbounded(nextPosition);

                var canMove = projectedFootprint.All(t => t != null && !t.IsOccupied);

                if (canMove)
                {
                    currTile = nextPosition;
                    moved++;
                }
                else
                {
                    break;
                }
            }

            if (moved > 0)
            {
                var destinationTile = tilemap.GetTile(currTile);
                pawn.MoveToPosition(destinationTile, null);
            }

            if (moved >= magnitude)
                return;

            var missing = magnitude - moved;
            pawn.Combat.ReceiveAttack(damagePerTile * missing);

            // Collision detection with multiple pawns
            var collisionFootprint = pawn.TilemapHelper.GenerateFootprintUnbounded(currTile + direction);
            var collidedPawns = collisionFootprint
                .Where(t => t != null && t.IsOccupied)
                .Select(t => t.Pawn)
                .Where(p => p != null && p != pawn) // avoid self
                .Distinct();

            foreach (var collidedPawn in collidedPawns) collidedPawn.Combat.ReceiveAttack(damagePerTile * missing);
        }


        public static List<Tile> ProjectFootprint(PawnController pawn, Vector2Int direction)
        {
            var next = pawn.TilemapHelper.AnchorTile.Position + direction;
            var footprint = pawn.TilemapHelper.GenerateFootprintUnbounded(next);

            var list = footprint.Where(t => t != null).ToList();
            return list;
        }

        /// <summary>
        ///     Helper function to get the direction for a Vector2Int
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        private static Vector2Int NormalizeTaxicab(Vector2Int v)
        {
            if (v == Vector2Int.zero)
                return Vector2Int.zero;

            var absX = Mathf.Abs(v.x);
            var absY = Mathf.Abs(v.y);

            if (absX >= absY && absX != 0)
                return new Vector2Int(v.x > 0 ? 1 : -1, 0);
            return new Vector2Int(0, v.y > 0 ? 1 : -1);
        }
    }
}