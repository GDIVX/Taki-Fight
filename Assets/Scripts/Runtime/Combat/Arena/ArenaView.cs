using Runtime.Selection;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Scripts.Runtime.Combat.Arena
{
    public class ArenaView : MonoBehaviour
    {
        [SerializeField] private TileView tilePrefab; // Prefab for the tile
        [SerializeField] private float tileSize = 1.0f; // Size of each tile
        [SerializeField] private float tileOffset = 0.1f; // Offset for the tile position
        [SerializeField] private float tilePadding = 0.1f; // Padding between tiles


        private Dictionary<Vector2Int, TileView> tileObjects = new();

        internal void CreateTiles(Tile[,] tiles)
        {
            for (int x = 0; x < tiles.GetLength(0); x++)
            {
                for (int y = 0; y < tiles.GetLength(1); y++)
                {
                    var tile = tiles[x, y];
                    var position = new Vector3(x * (tileSize + tilePadding), y * (tileSize + tilePadding), 0); 
                    var tileObject = Instantiate(tilePrefab, position, Quaternion.identity, transform);
                    tileObject.transform.localScale = new Vector3(tileSize, tileSize, 1); 
                    tileObjects[tile.Position] = tileObject;
                    tileObject.SetTile(tile); // Set the tile data in the TileView

                }
            }
        }


        public TileView GetTileObject(Vector2Int position)
        {
            if (tileObjects.TryGetValue(position, out var tileObject))
            {
                return tileObject;
            }
            return null;
        }

        public void Disable()
        {
            // Clear the arena view
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(false);
            }
        }

        public void Enable()
        {
            // Enable the arena view
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(true);
            }
        }

    }
}