using System;
using JetBrains.Annotations;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Runtime.Combat.Tilemap
{
    [Serializable]
    public class TilemapGenerator
    {
        [Button]
        public Tile[,] GenerateTilemap([NotNull] TilemapConfig config, [NotNull] TilemapView tilemapView,
            Action<Tile[,]> onComplete = null)
        {
            if (config == null) throw new ArgumentNullException(nameof(config));

            if (tilemapView == null) throw new ArgumentNullException(nameof(tilemapView));

            if (config.Columns == null || config.Columns.Count == 0)
            {
                throw new ArgumentException("TilemapConfig must have at least one TileOwner in the columns list.");
            }

            // Determine the size of the arena
            int rows = config.Rows;
            int cols = config.Columns.Count;

            // Create the tilemap
            var tiles = new Tile[cols, rows];

            for (int x = 0; x < cols; x++)
            {
                for (int y = 0; y < rows; y++)
                {
                    var position = new Vector2Int(x, y);
                    tiles[x, y] = new Tile(position);

                    // Assign the owner of the tile based on the columns list
                    tiles[x, y].Owner = config.Columns[x];
                }
            }

            // Create the tiles in the view
            tilemapView.CreateTiles(tiles);

            onComplete?.Invoke(tiles);
            return tiles;
        }
    }
}