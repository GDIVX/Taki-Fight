using Runtime.Combat.Pawn;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Runtime.Combat.Arena
{
    [Serializable]
    public class Tile
    {
        [ShowInInspector, ReadOnly] private Vector2Int position;
        [ShowInInspector, ReadOnly] private PawnController pawn;
        [ShowInInspector, ReadOnly] private TileOwner owner = TileOwner.None;
        [ShowInInspector, ReadOnly] private TileView view;

        public Vector2Int Position { get => position; private set => position = value; }
        public PawnController Pawn { get => pawn; private set => pawn = value; }
        public TileOwner Owner { get => owner; private set => owner = value; }
        public TileView View { get => view; internal set => view = value; }

        public bool IsOccupied => Pawn != null;
        public bool IsEmpty => Pawn == null;

        public Tile(Vector2Int position)
        {
            Position = position;
        }

        public void SetPawn(PawnController pawn)
        {
            Pawn = pawn;
        }

        public void SetOwner(TileOwner owner)
        {
            Owner = owner;
        }


        internal void Clear()
        {
            Pawn = null;
        }
    }

}
