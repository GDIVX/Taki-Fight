using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Runtime.Combat.Tilemap;
using Sirenix.OdinInspector;
using UnityEngine;
using Utilities;

namespace Runtime.Combat.Pawn
{
    [Serializable]
    public class PawnTilemapHelper
    {
        [ShowInInspector, ReadOnly] private Tile _anchorTile;
        [ShowInInspector, ReadOnly] private Vector2Int[] _footprint;

        public PawnTilemapHelper(Vector2Int size, PawnController pawn)
        {
            Size = size;
            Pawn = pawn ?? throw new ArgumentNullException(nameof(pawn));

            _footprint = GenerateFootprint(Size.x, Size.y);
        }


        public List<Tile> OccupiedTiles { get; set; } = new();

        public Tile AnchorTile
        {
            get => _anchorTile;
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

        private Vector2Int[] GenerateFootprint(int width, int height)
        {
            var list = new List<Vector2Int>();
            for (int x = 0; x < width; x++)
            for (var y = 0; y < height; y++)
                list.Add(new Vector2Int(x, y));
            return list.ToArray();
        }

        internal Tile[] GenerateFootprintUnbounded(Vector2Int anchor)
        {
            var tilemap = ServiceLocator.Get<TilemapController>();
            return tilemap.GenerateFootprintUnbounded(anchor, Size);
        }

        /// <summary>
        ///     Return the edges of the currently occupied tiles
        /// </summary>
        /// <remarks>
        ///     An edge tile is one of <see cref="OccupiedTiles" /> that has at least
        ///     one neighbour not occupied by this pawn.
        /// </remarks>
        /// <returns>List that may be empty but is never <c>null</c>.</returns>
        [ItemCanBeNull]
        public List<Tile> GetEdges()
        {
            // No footprint – no edges
            if (OccupiedTiles == null || OccupiedTiles.Count == 0)
                return new List<Tile>();

            // Speed-up look-ups
            var occupiedSet = new HashSet<Tile>(OccupiedTiles);

            return OccupiedTiles
                .Where(IsEdgeTile)
                .ToList();

            // A tile is an edge when at least one neighboring position is free
            bool IsEdgeTile(Tile tile)
            {
                return tile != null &&
                       tile.GetNeighbors()
                           .Any(neighbour => neighbour == null || !occupiedSet.Contains(neighbour));
            }
        }
    }
}