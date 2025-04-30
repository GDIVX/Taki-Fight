using System;
using System.Collections.Generic;
using Runtime.Combat.Pawn;
using UnityEngine;

namespace Runtime.Combat.Tilemap
{
    public class TilemapController
    {
        private Tile[,] _tiles;
        private TilemapView _view;
        private List<PawnController> _activeUnits = new();
        public TilemapView View { get => _view; private set => _view = value; }


        public TilemapController(int width, int height, TilemapView arenaView)
        {
            _tiles = new Tile[width, height];
            View = arenaView;
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    _tiles[x, y] = new Tile(new Vector2Int(x, y));
                }
            }

            View.CreateTiles(_tiles);
        }


        public void Clear()
        {
            foreach (var tile in _tiles)
            {
                tile.Clear();
            }

            View.Disable();
        }

        public void Enable()
        {
            View.Enable();
        }

        internal Tile? GetTile(Vector2Int position)
        {
            // Check if the position is within bounds
            if (position.x < 0 || position.x >= _tiles.GetLength(0) ||
                position.y < 0 || position.y >= _tiles.GetLength(1))
            {
                // Return null if the position is out of bounds
                return null;
            }

            // Return the tile at the specified position
            return _tiles[position.x, position.y];
        }

        internal List<PawnController> GetAllUnits()
        {
            return _activeUnits;
        }
        internal void AddUnit(PawnController unit)
        {
            if (!_activeUnits.Contains(unit))
            {
                _activeUnits.Add(unit);
            }
        }

        internal void RemoveUnit(PawnController unit)
        {
            _activeUnits.Remove(unit);
        }
    }
}