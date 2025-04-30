using Runtime.Selection;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Runtime.Combat.Tilemap
{
    public class TilemapView : MonoBehaviour
    {
        [SerializeField] private TileView tilePrefab; // Prefab for the tile
        [SerializeField] private float tileSize = 1.0f; // Size of each tile
        [SerializeField] private float tilePadding = 0.1f; // Padding between tiles

        private Dictionary<Vector2Int, TileView> tileObjects = new();

        internal void CreateTiles(Tile[,] tiles)
        {
            // Calculate the total width of the tilemap
            int cols = tiles.GetLength(0);
            float totalWidth = cols * tileSize + (cols - 1) * tilePadding;

            // Calculate the X offset to center the tilemap on the screen
            float screenCenterX = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width / 2, 0, 0)).x;
            float tilemapCenterX = totalWidth / 2;
            float offsetX = screenCenterX - tilemapCenterX;

            // Set the position of the TilemapView GameObject
            transform.position = new Vector3(offsetX, transform.position.y, transform.position.z);

            // Use the local position of the GameObject as the origin
            Vector3 origin = transform.localPosition;

            // Create the tiles
            for (int x = 0; x < tiles.GetLength(0); x++)
            {
                for (int y = 0; y < tiles.GetLength(1); y++)
                {
                    var tile = tiles[x, y];
                    var position = origin + new Vector3(x * (tileSize + tilePadding), y * (tileSize + tilePadding), 0);
                    var tileObject = Instantiate(tilePrefab, position, Quaternion.identity, transform);
                    tileObject.transform.localScale = new Vector3(tileSize, tileSize, 1);
                    tileObjects[tile.Position] = tileObject;

                    tileObject.Init(tile); // Set the tile data in the TileView
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

        public Vector2 MapToWorldPoint(Vector2Int position)
        {
            Vector3 origin = transform.localPosition; // Use the local position of the GameObject as the origin
            float x = origin.x + position.x * (tileSize + tilePadding);
            float y = origin.y + position.y * (tileSize + tilePadding);
            return new Vector2(x, y);
        }

        public Vector2Int WorldToMapPoint(Vector2 worldPosition)
        {
            Vector3 origin = transform.localPosition; // Use the local position of the GameObject as the origin
            int x = Mathf.FloorToInt((worldPosition.x - origin.x) / (tileSize + tilePadding));
            int y = Mathf.FloorToInt((worldPosition.y - origin.y) / (tileSize + tilePadding));
            return new Vector2Int(x, y);
        }
    }
}