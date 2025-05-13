using System;
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
            set
            {
                _owner = value;
                if (_view != null)
                {
                    _view.OnOwnerModified();
                }
            }
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
            _pawnController.Remove();
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
    }
}