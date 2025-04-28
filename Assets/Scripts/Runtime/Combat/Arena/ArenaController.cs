using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Runtime.Combat.Arena;
using Runtime.Combat.Pawn;
using UnityEngine;

namespace Runtime.Combat.Arena
{
    public class ArenaController
    {
        private Tile[,] _tiles;
        private ArenaView _arenaView;


        public ArenaController(int width, int height, ArenaView arenaView)
        {
            _tiles = new Tile[width, height];
            _arenaView = arenaView;
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    _tiles[x, y] = new Tile(new Vector2Int(x, y));
                }
            }

            _arenaView.CreateTiles(_tiles);
        }

        public void Clear()
        {
            foreach (var tile in _tiles)
            {
                tile.Clear();
            }

            _arenaView.Disable();
        }

        public void Enable()
        {
            _arenaView.Enable();
        }

        internal Tile GetTile(Vector2Int position)
        {
            throw new NotImplementedException();
        }
    }
}