﻿using System;
using Runtime.Combat.Tilemap;
using Sirenix.OdinInspector;
using UnityEngine;
using Utilities;

namespace Runtime.Combat.Pawn
{
    [Serializable]
    public class PawnMovement
    {
        public Observable<int> Speed;

        private PawnController _pawn;

        //Reference to the tilemap helper for ease of access
        PawnTilemapHelper _tilemapHelper;

        public PawnMovement(PawnController pawn, PawnTilemapHelper tilemapHelper, int speed)
        {
            _tilemapHelper = tilemapHelper;
            _pawn = pawn;
            Speed = new(speed);
        }

        [ShowInInspector] [ReadOnly] public int AvilableSpeed { get; private set; }

        internal void ResetSpeed()
        {
            AvilableSpeed = Speed.Value;
        }

        public bool TryToMove(Action onComplete)
        {
            if (AvilableSpeed <= 0) return false;

            var nextTile = GetNextTile();

            _pawn.ExecuteMoveStrategies(_pawn.Data.MovementAbilities, ref nextTile);

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


            _pawn.MoveToPosition(nextTile, () =>
            {
                AvilableSpeed--;

                //execute onMove strategies
                _pawn.ExecuteStrategies(_pawn.Data.OnMoveStrategies);
                onComplete?.Invoke();
            });
            return true;
        }

        private Tile GetNextTile()
        {
            var anchorTile = _tilemapHelper.AnchorTile;
            var tilemap = ServiceLocator.Get<TilemapController>();
            if (tilemap == null) return anchorTile;

            var nextAnchor = anchorTile.Position + GetMovementDirection();
            var nextTile = tilemap.GetTile(nextAnchor);
            if (nextTile == null) return anchorTile;

            // Check if the next tile is within bounds
            var footprint = _tilemapHelper.GenerateFootprintUnbounded(nextAnchor);
            var size = _tilemapHelper.Size;
            return footprint.Length != size.x * size.y ? anchorTile : nextTile;
        }

        private Vector2Int GetMovementDirection()
        {
            //player units goes right, enemies go left
            return _pawn.Owner == PawnOwner.Player ? Vector2Int.right : Vector2Int.left;
        }
    }
}