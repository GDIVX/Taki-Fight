using System.Collections.Generic;
using UnityEngine;
using Utilities;

namespace Runtime.Combat.Tilemap
{
    public class TilemapView : MonoBehaviour
    {
        [SerializeField] private TileView tilePrefab;
        [SerializeField] private float tileSize = 1.0f;
        [SerializeField] private float tilePadding = 0.1f;

        private readonly Dictionary<Vector2Int, TileView> tileObjects = new();

        public float TileSize => tileSize;

        private void OnDestroy()
        {
            var tilemapController = ServiceLocator.Get<TilemapController>();
            ServiceLocator.Unregister(tilemapController);
        }

        internal void CreateTiles(Tile[,] tiles)
        {
            var cols = tiles.GetLength(0);
            var totalWidth = cols * tileSize + (cols - 1) * tilePadding;

            var screenCenterX = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width / 2, 0, 0)).x;
            var tilemapCenterX = totalWidth / 2;
            var offsetX = screenCenterX - tilemapCenterX;

            // Set position of the tilemap in world space
            transform.position = new Vector3(offsetX, transform.position.y, transform.position.z);

            // Use world position as origin now
            var origin = transform.position;

            for (var x = 0; x < tiles.GetLength(0); x++)
            {
                for (var y = 0; y < tiles.GetLength(1); y++)
                {
                    var tile = tiles[x, y];
                    var position = origin + new Vector3(x * (tileSize + tilePadding), y * (tileSize + tilePadding), 0);
                    var tileObject = Instantiate(tilePrefab, position, Quaternion.identity, transform);
                    tileObject.transform.localScale = new Vector3(tileSize, tileSize, 1);
                    tileObjects[tile.Position] = tileObject;

                    tileObject.Init(tile);
                }
            }
        }

        public TileView GetTileObject(Vector2Int position)
        {
            tileObjects.TryGetValue(position, out var tileObject);
            return tileObject;
        }

        public void Disable()
        {
            foreach (Transform child in transform)
                child.gameObject.SetActive(false);
        }

        public void Enable()
        {
            foreach (Transform child in transform)
                child.gameObject.SetActive(true);
        }

        public Vector2 MapToWorldPoint(Vector2Int position)
        {
            var origin = transform.position;
            var x = origin.x + position.x * (tileSize + tilePadding);
            var y = origin.y + position.y * (tileSize + tilePadding);
            return new Vector2(x, y);
        }

        public Vector2Int WorldToMapPoint(Vector2 worldPosition)
        {
            var origin = transform.position;
            var x = Mathf.FloorToInt((worldPosition.x - origin.x) / (tileSize + tilePadding));
            var y = Mathf.FloorToInt((worldPosition.y - origin.y) / (tileSize + tilePadding));
            return new Vector2Int(x, y);
        }
    }
}