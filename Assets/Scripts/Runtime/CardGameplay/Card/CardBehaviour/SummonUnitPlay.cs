using System;
using System.Collections.Generic;
using Runtime.Combat.Tilemap;
using Runtime.Combat.Pawn;
using Runtime.Selection;
using UnityEngine;
using Utilities;

namespace Runtime.CardGameplay.Card.CardBehaviour
{
    [CreateAssetMenu(fileName = "Summon Unit", menuName = "Card/Strategy/Play/Summon Unit", order = 0)]
    public class SummonUnitPlay : CardPlayStrategy
    {
        [SerializeField] private PawnData _unit;
        [SerializeField] private TileSelectionMode _tileSelectionMode;

        public override void Play(CardController cardController, int potency, Action<bool> onComplete)
        {
            SelectionService.Instance.RequestSelection
            (
                target =>
                {
                    //cast the target to a tile
                    var tileView = target as TileView;
                    if (tileView == null)
                    {
                        Debug.LogError("SummonUnitPlay: Target is not a TileView.");
                        onComplete?.Invoke(false);
                        return false;
                    }
                    //get the size of the unit
                    var unitSize = _unit.Size;

                    //get all tiles for the footprint of the unit
                    var tilemap = ServiceLocator.Get<TilemapController>();
                    if (tilemap == null)
                    {
                        Debug.LogError("SummonUnitPlay: TilemapController not found.");
                        onComplete?.Invoke(false);
                        return false;
                    }

                    if (!tilemap.TryGenerateFootprintBounded(tileView.Tile.Position, unitSize, out var footprint))
                    {
                        onComplete?.Invoke(false);
                        return false;
                    }

                    //check if the footprint is valid
                    //iterate through the footprint and check validation
                    foreach (var tile in footprint)
                    {
                        if (tile == null)
                        {
                            onComplete?.Invoke(false);
                            return false;
                        }
                        //check if tile is in bounds of the tilemap
                        if (!tilemap.IsInBounds(tile.Position))
                        {
                            onComplete?.Invoke(false);
                            return false;
                        }
                        if (tile.IsOccupied)
                        {
                            onComplete?.Invoke(false);
                            return false;
                        }

                        //all tiles must adhear to the tile selection mode
                        if (!TileFilterHelper.FilterTile(tile, _tileSelectionMode))
                        {
                            onComplete?.Invoke(false);
                            return false;
                        }
                    }
                    //if all tiles are valid, return true
                    return true;
                },
                1,
                selectedEntities =>
                {
                    var tileView = selectedEntities[0] as TileView;
                    var tile = tileView.Tile;

                    if (tile.IsOccupied)
                    {
                        onComplete?.Invoke(true); // graceful fail
                        return;
                    }

                    var pawnFactory = ServiceLocator.Get<PawnFactory>();
                    if (pawnFactory == null)
                    {
                        Debug.LogError("SummonUnitPlay: PawnFactory not found.");
                        onComplete?.Invoke(false);
                        return;
                    }

                    var unit = _unit;
                    pawnFactory.CreatePawn(unit, tile);
                    onComplete?.Invoke(true);
                },
                () =>
                {
                    onComplete?.Invoke(false);
                },
                cardController.transform.position
            );
        }
    }
}


