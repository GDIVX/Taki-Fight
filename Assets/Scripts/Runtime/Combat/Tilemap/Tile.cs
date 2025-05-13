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
        [ShowInInspector, ReadOnly] private TileOwner owner;
        [ShowInInspector] [ReadOnly] private PawnController pawn;
        [ShowInInspector] [ReadOnly] private Vector2Int position;
        [ShowInInspector, ReadOnly] private TileView view;

        public Tile(Vector2Int position)
        {
            this.position = position;
            pawn = null;
            owner = TileOwner.None;
            view = null;
        }

        public Vector2Int Position
        {
            get => position;
            private set => position = value;
        }

        public PawnController Pawn
        {
            get => pawn;
            private set => pawn = value;
        }

        public TileOwner Owner
        {
            get => owner;
            set
            {
                owner = value;
                if (view != null)
                {
                    view.OnOwnerModified();
                }
            }
        }

        public TileView View
        {
            get => view;
            internal set => view = value;
        }

        public bool IsOccupied => Pawn != null;
        public bool IsEmpty => Pawn == null;

        public bool Equals(Tile other)
        {
            if (other == null) return false;

            return position.Equals(other.position);
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
            pawn.Remove();
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
            return HashCode.Combine(position);
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