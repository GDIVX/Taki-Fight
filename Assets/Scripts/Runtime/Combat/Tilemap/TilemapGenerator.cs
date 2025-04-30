using System;
using Runtime.Combat.Tilemap;
using UnityEngine;

namespace Runtime.Combat.Tilemap
{
    [Serializable]
    public static class TilemapGenerator
    {
        public static Tile[,] GenerateTilemap(Vector2Int arenaSize, TilemapView tilemapView)
        {
            var tiles = new Tile[arenaSize.x, arenaSize.y];

            for (int x = 0; x < arenaSize.x; x++)
            {
                for (int y = 0; y < arenaSize.y; y++)
                {
                    var position = new Vector2Int(x, y);
                    tiles[x, y] = new Tile(position);

                    // Set the owner of the tile based on its position

                }
            }



            // Create the tiles in the view
            tilemapView.CreateTiles(tiles);

            return tiles;
        }
    }
}
