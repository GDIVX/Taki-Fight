using Runtime.Combat.Tilemap;
using UnityEngine;
using Utilities;

namespace Runtime.Combat.Pawn.Abilities
{
    [CreateAssetMenu(fileName = "Fly Ability", menuName = "Pawns/Abilities/Movement/Fly", order = 0)]
    public class FlyAbility : PawnMovePlayStrategy
    {
        public override void ModifyMove(PawnController pawn, ref Tile nextTile)
        {
            var tilemap = ServiceLocator.Get<TilemapController>();
            if (tilemap == null || nextTile == null) return;

            var direction = pawn.Owner == PawnOwner.Player ? Vector2Int.right : Vector2Int.left;
            var remaining = Potency <= 0 ? int.MaxValue : Potency;

            var candidate = nextTile;
            while (candidate != null && candidate.IsOccupied && remaining > 0)
            {
                var forwardPos = candidate.Position + direction;
                candidate = tilemap.GetTile(forwardPos);
                remaining--;
            }

            if (candidate != null)
            {
                nextTile = candidate;
            }
        }

        public override string GetDescription()
        {
            return Potency > 0 ? $"Fly over up to {Potency} tiles" : "Fly";
        }
    }
}
