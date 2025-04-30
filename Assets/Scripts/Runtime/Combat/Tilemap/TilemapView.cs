using Runtime.Combat.Tilemap;
using System.Collections.Generic;
using UnityEngine;

public class TilemapView : MonoBehaviour
{
    [SerializeField] private TileView tilePrefab;
    [SerializeField] private float tileSize = 1.0f;
    [SerializeField] private float tilePadding = 0.1f;

    private Dictionary<Vector2Int, TileView> tileObjects = new();

    public float TileSize => tileSize;

    internal void CreateTiles(Tile[,] tiles)
    {
        int cols = tiles.GetLength(0);
        float totalWidth = cols * tileSize + (cols - 1) * tilePadding;

        float screenCenterX = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width / 2, 0, 0)).x;
        float tilemapCenterX = totalWidth / 2;
        float offsetX = screenCenterX - tilemapCenterX;

        // Set position of the tilemap in world space
        transform.position = new Vector3(offsetX, transform.position.y, transform.position.z);

        // Use world position as origin now
        Vector3 origin = transform.position;

        for (int x = 0; x < tiles.GetLength(0); x++)
        {
            for (int y = 0; y < tiles.GetLength(1); y++)
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
        Vector3 origin = transform.position;
        float x = origin.x + position.x * (tileSize + tilePadding);
        float y = origin.y + position.y * (tileSize + tilePadding);
        return new Vector2(x, y);
    }

    public Vector2Int WorldToMapPoint(Vector2 worldPosition)
    {
        Vector3 origin = transform.position;
        int x = Mathf.FloorToInt((worldPosition.x - origin.x) / (tileSize + tilePadding));
        int y = Mathf.FloorToInt((worldPosition.y - origin.y) / (tileSize + tilePadding));
        return new Vector2Int(x, y);
    }
}
