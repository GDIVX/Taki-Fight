using Runtime.Combat.Tilemap;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;
using Utilities;

namespace Runtime.Combat.Pawn
{
    [Serializable]
    public class PawnTilemapHelper
    {
        [ShowInInspector, ReadOnly] private Tile _anchorTile;
        [ShowInInspector, ReadOnly] private Vector2Int[] _footprint;


        public List<Tile> OccupiedTiles { get; set; } = new List<Tile>();
        public Tile AnchorTile
        {
            get
            {
                return _anchorTile;
            }
            set
            {
                _anchorTile = value;
                if (_anchorTile != null)
                {
                    _anchorTile.SetPawn(Pawn);
                }
            }
        }

        public Vector2Int Size { get; private set; }

        public PawnController Pawn { get; private set; }

        public PawnTilemapHelper(Vector2Int size, PawnController pawn)
        {
            Size = size;
            Pawn = pawn ?? throw new ArgumentNullException(nameof(pawn));

            _footprint = GenerateFootprint(Size.x, Size.y);


        }

        private Vector2Int[] GenerateFootprint(int width, int height)
        {
            var list = new List<Vector2Int>();
            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                    list.Add(new Vector2Int(x, y));
            return list.ToArray();
        }

        internal Tile[] GenerateFootprintUnbounded(Vector2Int anchor)
        {
            var tilemap = ServiceLocator.Get<TilemapController>();
            return tilemap.GenerateFootprintUnbounded(anchor, Size);
        }
    }
}
