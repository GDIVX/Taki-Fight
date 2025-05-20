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
        private float RealSize => tileSize + tilePadding;

        private void OnDestroy()
        {
            var tilemapController = ServiceLocator.Get<TilemapController>();
            ServiceLocator.Unregister(tilemapController);
        }

        internal void CreateTiles(Tile[,] tiles)
        {
            var cols = tiles.GetLength(0);
            var rows = tiles.GetLength(1);

            // Use world position as origin now
            var origin = transform.position;

            for (var x = 0; x < cols; x++)
            {
                for (var y = 0; y < rows; y++)
                {
                    var tile = tiles[x, y];
                    var position = origin + new Vector3(x * RealSize, y * RealSize, 0);
                    var tileObject = Instantiate(tilePrefab, position, Quaternion.identity, transform);
                    tileObject.transform.localScale = new Vector3(RealSize, RealSize, 1);
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
            var x = origin.x + position.x * RealSize;
            var y = origin.y + position.y * RealSize;
            return new Vector2(x, y);
        }

        public Vector2Int WorldToMapPoint(Vector2 worldPosition)
        {
            var origin = transform.position;
            var x = Mathf.FloorToInt((worldPosition.x - origin.x) / RealSize);
            var y = Mathf.FloorToInt((worldPosition.y - origin.y) / RealSize);
            return new Vector2Int(x, y);
        }
    }
}