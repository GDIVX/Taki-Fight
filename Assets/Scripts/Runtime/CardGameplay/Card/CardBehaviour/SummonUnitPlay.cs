using System;
using System.Collections.Generic;
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
        private TileFilterCriteria TileSelectionMode => _params.TileFilter;

        public override void Play(CardController cardController, Action<bool> onComplete)
        {
            SelectionService.Instance.SearchSize = Pawn.Size;

            SelectionService.Instance.RequestSelection
            (
                IsValidTile,
                1,
                SummonPawn,
                () => { onComplete?.Invoke(false); },
                cardController.transform.position
            );
            return;

            void SummonPawn(List<ISelectableEntity> selectedEntities)
            {
                var tileView = selectedEntities[0] as TileView;
                if (tileView)
                {
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

                    //if the card instance already holds a pawn instance, we use it to spawn. Otherwise, we spawn a new pawn

                    PawnController controller;
                    if (cardController.Instance.PawnInstant == null)
                    {
                        var unit = Pawn;
                        controller = pawnFactory.CreatePawn(unit, tile);
                    }
                    else
                    {
                        var unit = cardController.Instance.PawnInstant;
                        controller = pawnFactory.CreatePawn(unit, tile);
                    }


                    //destroy the card for now. It would be recreated later if needed
                    controller.BoundCard(cardController.Instance);
                    cardController.SetAside();
                }

                onComplete?.Invoke(true);
            }

            bool IsValidTile(ISelectableEntity target)
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
                    if (!TileFilterHelper.FilterTile(tile, TileSelectionMode))
                    {
                        onComplete?.Invoke(false);
                        return false;
                    }
                }

                //if all tiles are valid, return true
                return true;
            }
        }

        public override string GetDescription()
        {
            var descriptionBuilder = new DescriptionBuilder();
            return descriptionBuilder.AsSummon(Pawn);
        }

        public override void Initialize(PlayStrategyData playStrategyData)
        {
            _params = playStrategyData.Parameters as SummonUnitParams;
            Pawn.InitializeStrategies();
            base.Initialize(playStrategyData);
        }
    }
}