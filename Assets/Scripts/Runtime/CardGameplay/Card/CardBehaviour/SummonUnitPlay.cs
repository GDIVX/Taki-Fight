using System;
using Runtime.Combat.Pawn;
using Runtime.Combat.Tilemap;
using Runtime.Selection;
using UnityEngine;
using Utilities;

namespace Runtime.CardGameplay.Card.CardBehaviour
{
    [CreateAssetMenu(fileName = "Summon Unit", menuName = "Card/Strategy/Play/Summon Unit", order = 0)]
    public class SummonUnitPlay : CardPlayStrategy
    {
        private SummonUnitParams _params;

        internal PawnData Pawn => _params.Unit;

        public override void Play(CardController cardController, Action<bool> onComplete)
        {
            SelectionService.Instance.SearchSize = Pawn.Size;

            SelectionService.Instance.RequestSelection
            (
                target =>
                {
                    //cast the target to a tile
                    var tileView = target as TileView;
                    if (tileView == null)
                    {
                        // Debug.LogError("SummonUnitPlay: Target is not a TileView.");
                        onComplete?.Invoke(false);
                        return false;
                    }

                    //get the size of the unit
                    var unitSize = Pawn.Size;

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
                        var tileSelectionMode = new TileFilterCriteria
                        {
                            Occupancy = OccupancyFilter.Empty,
                            TileOwner = TileOwner.Player
                        };
                        if (!TileFilterHelper.FilterTile(tile, tileSelectionMode))
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

                    var unit = Pawn;
                    var pawnController = pawnFactory.CreatePawn(unit, tile);

                    pawnController.AssignSummonCard(cardController);

                    onComplete?.Invoke(true);
                },
                () => { onComplete?.Invoke(false); },
                cardController.transform.position
            );
        }

        public override string GetDescription()
        {
            var descriptionBuilder = new DescriptionBuilder();
            return descriptionBuilder.AsSummon(Pawn);
        }

        public override void Initialize(PlayStrategyData playStrategyData, CardController cardController)
        {
            _params = playStrategyData.Parameters as SummonUnitParams;
            Pawn.InitializeStrategies();
            base.Initialize(playStrategyData, cardController);
        }
    }
}