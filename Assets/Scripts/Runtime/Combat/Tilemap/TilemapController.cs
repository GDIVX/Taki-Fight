using Runtime.Combat.Pawn;
using Runtime.Combat.Tilemap;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class TilemapController
{
    [ShowInInspector] private Tile[,] _tiles;
    private TilemapView _view;
    private List<PawnController> _activeUnits = new();
    public TilemapView View { get => _view; private set => _view = value; }

    public TilemapController(Tile[,] tiles, TilemapView arenaView)
    {
        _tiles = tiles;
        View = arenaView;
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
        // Check if the position is within the bounds of the tile array
        if (position.x < 0 || position.x >= _tiles.GetLength(0) || position.y < 0 || position.y >= _tiles.GetLength(1))
        {
            return null; // Return null if the position is out of bounds
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


    /// <summary>
    /// generate a list of all tiles that are within the given size and anchored at the given position
    /// </summary>
    /// <param name="anchor"></param>
    /// <param name="size"></param>
    /// <returns></returns>
    internal Tile[] GenerateFootprintUnbounded(Vector2Int anchor, Vector2Int size)
    {
        var footprint = new List<Tile>();
        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                var tile = GetTile(anchor + new Vector2Int(x, y));
                if (tile != null)
                {
                    footprint.Add(tile);
                }
            }
        }
        return footprint.ToArray();
    }

    public bool TryGenerateFootprintBounded(Vector2Int anchor, Vector2Int size, out Tile[] footprint)
    {
        footprint = GenerateFootprintUnbounded(anchor, size);
        // Check if the footprint is valid (i.e., all tiles are not null)
        if (footprint.Length == size.x * size.y)
        {
            return true; // Footprint is valid
        }
        else
        {
            footprint = null; // Footprint is invalid
            return false;
        }
    }

    internal bool IsInBounds(Vector2Int position)
    {
        // Check if the position is within the bounds of the tile array
        return position.x >= 0 && position.x < _tiles.GetLength(0) && position.y >= 0 && position.y < _tiles.GetLength(1);
    }

    internal List<Tile> GetEnemyOwnedTiles()
    {
        var enemyOwnedTiles = new List<Tile>();
        foreach (var tile in _tiles)
        {
            if (tile.Owner == TileOwner.Enemy)
            {
                enemyOwnedTiles.Add(tile);
            }
        }

        return enemyOwnedTiles;
    }
}
