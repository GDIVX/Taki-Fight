using System;
using System.Collections.Generic;
using Runtime.Combat.Pawn;
using Sirenix.OdinInspector;
using UnityEngine;
using Utilities;

namespace Runtime.Combat.Tilemap
{
    [Serializable]
    public class Tile : IEquatable<Tile>
    {
        [ShowInInspector] [ReadOnly] private TileOwner _owner;
        [ShowInInspector] [ReadOnly] private PawnController _pawnController;
        [ShowInInspector] [ReadOnly] private Vector2Int _position;
        [ShowInInspector] [ReadOnly] private TileView _view;

        public Tile(Vector2Int position)
        {
            _position = position;
            _pawnController = null;
            _owner = TileOwner.None;
            _view = null;
        }

        public Vector2Int Position
        {
            get => _position;
            private set => _position = value;
        }

        public PawnController Pawn
        {
            get => _pawnController;
            private set => _pawnController = value;
        }

        public TileOwner Owner
        {
            get => _owner;
            internal set => _owner = value;
        }

        public TileView View
        {
            get => _view;
            internal set => _view = value;
        }

        public bool IsOccupied => Pawn != null;
        public bool IsEmpty => Pawn == null;

        public bool Equals(Tile other)
        {
            if (other == null) return false;

            return _position.Equals(other._position);
        }

        public void SetPawn(PawnController pawn)
        {
            if (Pawn != null)
            {
                // Notify the TilemapController to remove the unit
                ServiceLocator.Get<TilemapController>()?.RemoveUnit(Pawn);
            }

            Pawn = pawn;

            if (Pawn != null)
            {
                // Notify the TilemapController to add the unit
                ServiceLocator.Get<TilemapController>()?.AddUnit(Pawn);
            }
        }


        internal void Clear()
        {
            if (!Pawn) return;
            _pawnController.Remove(false);
            Pawn = null;
        }

        public override bool Equals(object obj)
        {
            return obj is Tile tile && Equals(tile);
        }


        internal void SetView(TileView tileView)
        {
            if (tileView == null)
            {
                Debug.LogError("TileView is null");
                return;
            }

            View = tileView;
        }


        public override int GetHashCode()
        {
            return HashCode.Combine(_position);
        }

        public static bool operator ==(Tile left, Tile right)
        {
            if (left is null && right is null)
            {
                return true;
            }

            return left.Equals(right);
        }

        public static bool operator !=(Tile left, Tile right)
        {
            if (left is null && right is null)
            {
                return false;
            }

            return !(left == right);
        }

        public IEnumerable<Tile> GetNeighbors()
        {
            // Define all 8 possible directions (cardinal + diagonal)
            var directions = new[]
            {
                new Vector2Int(-1, 0), // Left
                new Vector2Int(1, 0), // Right
                new Vector2Int(0, -1), // Down
                new Vector2Int(0, 1), // Up
                new Vector2Int(-1, -1), // Down-Left
                new Vector2Int(-1, 1), // Up-Left
                new Vector2Int(1, -1), // Down-Right
                new Vector2Int(1, 1) // Up-Right
            };

            var tilemap = ServiceLocator.Get<TilemapController>();
            if (tilemap == null) yield break;

            // Check each direction and return valid neighboring tiles
            foreach (var dir in directions)
            {
                var neighborPos = Position + dir;
                var neighborTile = tilemap.GetTile(neighborPos);

                if (neighborTile != null) yield return neighborTile;
            }
        }

        public void SetOwner(TileOwner tileOwner)
        {
            Owner = tileOwner;
            _view.OnOwnerModified();
        }
    }
}
