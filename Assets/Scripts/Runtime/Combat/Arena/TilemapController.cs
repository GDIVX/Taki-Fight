using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Runtime.Combat.Tilemap;
using Runtime.Combat.Pawn;
using UnityEngine;

namespace Runtime.Combat.Tilemap
{
    public class TilemapController
    {
        private Tile[,] _tiles;
        private TilemapView _view;
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

        internal Tile GetTile(Vector2Int position)
        {
            throw new NotImplementedException();
        }
    }
}