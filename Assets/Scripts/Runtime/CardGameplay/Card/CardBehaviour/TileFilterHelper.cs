using Assets.Scripts.Runtime.Combat.Tilemap;
using System;

namespace Runtime.CardGameplay.Card.CardBehaviour
{
    internal class TileFilterHelper
    {
        internal static bool FilterTile(Tile tile, TileSelectionMode tileSelectionMode)
        {
            return tileSelectionMode switch
            {
                TileSelectionMode.All => true,
                TileSelectionMode.Empty => tile.IsEmpty,
                TileSelectionMode.Occupied => tile.IsOccupied,
                TileSelectionMode.EnemyOccupied => tile.IsOccupied && tile.Owner == TileOwner.Enemy,
                TileSelectionMode.FriendlyOccupied => tile.IsOccupied && IsFriendly(tile),
                TileSelectionMode.FriendlyEmpty => tile.IsEmpty && IsFriendly(tile),
                TileSelectionMode.EnemyEmpty => tile.IsEmpty && tile.Owner == TileOwner.Enemy,
                _ => throw new ArgumentOutOfRangeException(nameof(tileSelectionMode), tileSelectionMode, null),
            };
        }

        private static bool IsFriendly(Tile tile)
        {
            return
            (
            tile.Owner == TileOwner.Player ||
            tile.Owner == TileOwner.Heartland ||
            tile.Owner == TileOwner.castle
            );
        }
    }

    public enum TileSelectionMode
    {
        All,
        Empty,
        Occupied,
        EnemyOccupied,
        FriendlyOccupied,
        FriendlyEmpty,
        EnemyEmpty
    }
}