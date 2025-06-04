using System;
using Runtime.Combat.Pawn;
using Runtime.Combat.Tilemap;
using Sirenix.OdinInspector;

namespace Runtime.CardGameplay.Card.CardBehaviour
{
    public enum OccupancyFilter
    {
        All,
        Empty,
        Occupied
    }

    [Serializable]
    public struct TileFilterCriteria
    {
        [EnumToggleButtons] public OccupancyFilter Occupancy;

        [EnumToggleButtons] public TileOwner TileOwner;

        [ShowIf("@Occupancy == OccupancyFilter.Occupied")] [EnumToggleButtons]
        public PawnOwner PawnOwner;
    }

    internal static class TileFilterHelper
    {
        internal static bool FilterTile(Tile tile, TileFilterCriteria criteria)
        {
            // 1) occupancy check
            if (criteria.Occupancy == OccupancyFilter.Empty && tile.IsOccupied) return false;
            if (criteria.Occupancy == OccupancyFilter.Occupied && tile.IsEmpty) return false;


            // 2) tile‐owner check
            if (criteria.TileOwner != TileOwner.All && tile.Owner != criteria.TileOwner) return false;

            // 3) pawn‐owner check (only when occupied)
            return criteria.Occupancy != OccupancyFilter.Occupied
                   || !tile.IsOccupied
                   || tile.Pawn.Owner == criteria.PawnOwner;
        }
    }
}