using Runtime.Selection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;

namespace Runtime.Combat.Tilemap
{
    public static class AOEHighlight
    {

        static List<Tile> _currentlyHighlightedTiles = new List<Tile>();
        public static void HighlightForSelection(Tile tile)
        {
            var predicate = SelectionService.Instance.Predicate;

            var tilemap = ServiceLocator.Get<TilemapController>();
            var selectionSize = SelectionService.Instance.SearchSize;
            var color = predicate.Invoke(tile.View) ? Color.green : Color.red; // Change color based on selection predicate
            var footprint = tilemap.GenerateFootprintUnbounded(tile.Position, selectionSize);
            foreach (var t in footprint)
            {
                if (t == null) continue; // Skip null tiles

                var tileView = t.View;
                tileView.Highlight(color); // Highlight the tile based on the predicate
                _currentlyHighlightedTiles.Add(t); // Add to the list of highlighted tiles
            }
        }

        public static void ClearHighlights()
        {
            foreach (var tile in _currentlyHighlightedTiles)
            {
                tile.View.ClearHighlight(); // Clear the highlight for each tile
            }
            _currentlyHighlightedTiles.Clear(); // Clear the list after clearing highlights
        }

    }
}