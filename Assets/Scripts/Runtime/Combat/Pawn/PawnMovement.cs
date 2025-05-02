using Runtime.Combat.Pawn;
using Runtime.Combat.Tilemap;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using UnityEngine;
using Utilities;

namespace Assets.Scripts.Runtime.Combat.Pawn
{
    [Serializable]
    public class PawnMovement
    {
        //Reference to the tilemap helper for ease of access
        PawnTilemapHelper _tilemapHelper;
        PawnController _pawn;

        public TrackedProperty<int> Speed;
        [ShowInInspector, ReadOnly] public Vector2Int MovementDirection { get; set; }
        [ShowInInspector, ReadOnly] public int AvilableSpeed { get; private set; }

        public PawnMovement(PawnController pawn, PawnTilemapHelper tilemapHelper, int speed)
        {
            _tilemapHelper = tilemapHelper;
            _pawn = pawn;
            Speed = new(speed);
        }

        internal void ResetSpeed()
        {
            AvilableSpeed = Speed.Value;
        }

        public bool TryToMove()
        {
            if (AvilableSpeed <= 0) return false;

            var nextTile = GetNextTile();

            if (nextTile == null || nextTile == _tilemapHelper.AnchorTile) return false;

            // Check if the next tile or any tile in the footprint is occupied by another unit
            var size = _tilemapHelper.Size;
            var tilemap = ServiceLocator.Get<TilemapController>();

            if (tilemap == null)
            {
                Debug.LogError("TilemapController is not available.");
                return false;
            }

            var footprint = tilemap.GenerateFootprintUnbounded(nextTile.Position, size);

            foreach (var tile in footprint)
            {
                // Exclude tiles currently occupied by this unit
                if (_tilemapHelper.OccupiedTiles.Contains(tile)) continue;

                if (tile.IsOccupied)
                {
                    return false;
                }
                // Check ownership rules
                if (_pawn.Owner == PawnOwner.Player && nextTile.Owner == TileOwner.Enemy) return false;
            }


            _pawn.MoveToPosition(nextTile);
            AvilableSpeed--;

            return true;
        }

        private Tile GetNextTile()
        {
            var anchorTile = _tilemapHelper.AnchorTile;
            var tilemap = ServiceLocator.Get<TilemapController>();
            if (tilemap == null) return anchorTile;

            var nextAnchor = anchorTile.Position + MovementDirection;
            var nextTile = tilemap.GetTile(nextAnchor);
            if (nextTile == null) return anchorTile;

            // Check if the next tile is within bounds
            var footprint = _tilemapHelper.GenerateFootprintUnbounded(nextAnchor);
            var size = _tilemapHelper.Size;
            if (footprint.Length != size.x * size.y) return anchorTile;

            return nextTile;
        }
    }
}