using Assets.Scripts.Runtime.Combat.Arena;
using System;

namespace Runtime.CardGameplay.Card.CardBehaviour
{
    internal class TileFilterHelper
    {
        internal static bool FilterTile(Tile tile, TileSelectionMode tileSelectionMode)
        {
            switch (tileSelectionMode)
            {
                case TileSelectionMode.All:
                    return true;
                case TileSelectionMode.Empty:
                    return tile.IsEmpty;
                case TileSelectionMode.Occupied:
                    return tile.IsOccupied;
                case TileSelectionMode.EnemyOccupied:
                    return tile.IsOccupied && tile.Owner == TileOwner.Enemy;
                case TileSelectionMode.FriendlyOccupied:
                    return tile.IsOccupied && IsFriendly(tile);
                case TileSelectionMode.FriendlyEmpty:
                    return tile.IsEmpty && IsFriendly(tile);
                case TileSelectionMode.EnemyEmpty:
                    return tile.IsEmpty && tile.Owner == TileOwner.Enemy;
                default:
                    throw new ArgumentOutOfRangeException(nameof(tileSelectionMode), tileSelectionMode, null);
            }
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