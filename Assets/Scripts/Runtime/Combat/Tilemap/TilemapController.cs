using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Runtime.Combat.Pawn;
using Sirenix.OdinInspector;
using UnityEngine;
using Utilities;

namespace Runtime.Combat.Tilemap
{
    [Serializable]
    public class TilemapController : Service<TilemapController>
    {
        private List<PawnController> _activeUnits = new();
        [ShowInInspector] private Tile[,] _tiles;
        private TilemapView _view;

        public TilemapController([NotNull] Tile[,] tiles, [NotNull] TilemapView arenaView)
        {
            _tiles = tiles ?? throw new ArgumentNullException(nameof(tiles));
            View = arenaView ?? throw new ArgumentNullException(nameof(arenaView));
        }

        [NotNull]
        public TilemapView View
        {
            get => _view;
            private set => _view = value;
        }

        public void Clear()
        {
            foreach (var tile in _tiles) tile.Clear();

            View.Disable();
        }

        public void Enable()
        {
            View.Enable();
        }

        internal Tile GetTile(Vector2Int position)
        {
            // Check if the position is within the bounds of the tile array
            if (position.x < 0 || position.x >= _tiles.GetLength(0) || position.y < 0 ||
                position.y >= _tiles.GetLength(1))
                return null; // Return null if the position is out of bounds

            // Return the tile at the specified position
            return _tiles[position.x, position.y];
        }

        internal List<PawnController> GetAllUnits()
        {
            return _activeUnits;
        }

        internal void AddUnit(PawnController unit)
        {
            if (!_activeUnits.Contains(unit)) _activeUnits.Add(unit);
        }

        internal void RemoveUnit(PawnController unit)
        {
            _activeUnits.Remove(unit);
        }


        /// <summary>
        ///     generate a list of all tiles that are within the given size and anchored at the given position
        /// </summary>
        /// <param name="anchor"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        internal Tile[] GenerateFootprintUnbounded(Vector2Int anchor, Vector2Int size)
        {
            var footprint = new List<Tile>();
            for (var x = 0; x < size.x; x++)
            for (var y = 0; y < size.y; y++)
            {
                var tile = GetTile(anchor + new Vector2Int(x, y));
                if (tile != null) footprint.Add(tile);
            }

            return footprint.ToArray();
        }

        public bool TryGenerateFootprintBounded(Vector2Int anchor, Vector2Int size, out Tile[] footprint)
        {
            footprint = GenerateFootprintUnbounded(anchor, size);
            // Check if the footprint is valid (i.e., all tiles are not null)
            if (footprint.Length == size.x * size.y) return true; // Footprint is valid

            footprint = null; // Footprint is invalid
            return false;
        }

        internal bool IsInBounds(Vector2Int position)
        {
            // Check if the position is within the bounds of the tile array
            return position.x >= 0 && position.x < _tiles.GetLength(0) && position.y >= 0 &&
                   position.y < _tiles.GetLength(1);
        }

        internal List<Tile> GetEnemyOwnedTiles()
        {
            var enemyOwnedTiles = new List<Tile>();
            foreach (var tile in _tiles)
                if (tile.Owner == TileOwner.Enemy)
                    enemyOwnedTiles.Add(tile);

            return enemyOwnedTiles;
        }


        public IList<Tile> GetTilesInRange([NotNull] Tile anchor, int range, bool useDiagonals = false)
        {
            List<Tile> tilesInRange = new();

            var anchorPosition = anchor.Position;

            // Loop through all possible positions in the range
            for (var x = -range; x <= range; x++)
            for (var y = -range; y <= range; y++)
            {
                // Calculate the position relative to the anchor
                var currentPosition = anchorPosition + new Vector2Int(x, y);

                // Check distance based on diagonal or manhattan
                var distance = useDiagonals
                    ? Mathf.Max(Mathf.Abs(x), Mathf.Abs(y))
                    : // Chebyshev distance for diagonals
                    Mathf.Abs(x) + Mathf.Abs(y); // Manhattan distance

                if (distance <= range && IsInBounds(currentPosition))
                {
                    var tile = GetTile(currentPosition);
                    if (tile != null) tilesInRange.Add(tile);
                }
            }

            return tilesInRange;
        }

        public List<PawnController> GetUnitsByOwnership(PawnOwner owner)
        {
            return _activeUnits.Where(u => u.Owner == owner).ToList();
        }

        public List<Tile> AllTiles()
        {
            return _tiles.Cast<Tile>().ToList();
        }
    }
}