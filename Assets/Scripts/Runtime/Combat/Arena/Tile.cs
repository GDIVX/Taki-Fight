using Runtime.Combat.Pawn;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Utilities;

namespace Runtime.Combat.Tilemap
{
    [Serializable]
    public struct Tile : IEquatable<Tile>
    {
        [ShowInInspector, ReadOnly] private Vector2Int position;
        [ShowInInspector, ReadOnly] private PawnController pawn;
        [ShowInInspector, ReadOnly] private TileOwner owner;
        [ShowInInspector, ReadOnly] private TileView view;

        public Vector2Int Position { get => position; private set => position = value; }
        public PawnController Pawn { get => pawn; private set => pawn = value; }
        public TileOwner Owner { get => owner; private set => owner = value; }
        public TileView View { get => view; internal set => view = value; }

        public bool IsOccupied => Pawn != null;
        public bool IsEmpty => Pawn == null;

        public Tile(Vector2Int position)
        {
            this.position = position;
            this.pawn = null;
            this.owner = TileOwner.None;
            this.view = null;
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

        public void SetOwner(TileOwner owner)
        {
            Owner = owner;
        }

        internal void Clear()
        {
            Pawn = null;
        }

        public override bool Equals(object obj)
        {
            return obj is Tile tile && Equals(tile);
        }

        public bool Equals(Tile other)
        {
            return position.Equals(other.position);
        }

        public override readonly int GetHashCode()
        {
            return HashCode.Combine(position, owner);
        }

        public static bool operator ==(Tile left, Tile right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Tile left, Tile right)
        {
            return !(left == right);
        }
    }

}
