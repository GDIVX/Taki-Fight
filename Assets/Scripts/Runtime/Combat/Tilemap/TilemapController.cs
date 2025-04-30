using Runtime.Combat.Pawn;
using Runtime.Combat.Tilemap;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class TilemapController
{
    [ShowInInspector]private Tile[,] _tiles;
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
}
