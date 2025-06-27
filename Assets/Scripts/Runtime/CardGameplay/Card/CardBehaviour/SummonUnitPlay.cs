using System;
using System.Linq;
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
                    if (tileView) return IsValidTile(tileView.Tile);
                    // Debug.LogError("SummonUnitPlay: Target is not a TileView.");
                    onComplete?.Invoke(false);
                    return false;
                },
                1,
                selectedEntities =>
                {
                    var tileView = selectedEntities[0] as TileView;
                    var tile = tileView.Tile;

                    Summon(cardController, onComplete, tile);
                },
                () => { onComplete?.Invoke(false); },
                cardController.transform.position
            );
        }

        private static TileFilterCriteria TileFilterCriteria()
        {
            var tileSelectionMode = new TileFilterCriteria
            {
                Occupancy = OccupancyFilter.Empty,
                TileOwner = TileOwner.Player
            };
            return tileSelectionMode;
        }

        public override bool IsValidTile(Tile tile)
        {
            //get all tiles for the footprint of the unit
            var tilemap = ServiceLocator.Get<TilemapController>();
            if (tilemap == null)
            {
                Debug.LogError("SummonUnitPlay: TilemapController not found.");
                return false;
            }

            //get the size of the unit
            var unitSize = Pawn.Size;
            if (!tilemap.TryGenerateFootprintBounded(tile.Position, unitSize, out var footprint))
            {
                return false;
            }

            //check if the footprint is valid
            //iterate through the footprint and check validation
            foreach (var t in footprint)
            {
                if (t == null)
                {
                    return false;
                }

                //check if tile is in bounds of the tilemap
                if (!tilemap.IsInBounds(t.Position))
                {
                    return false;
                }

                if (t.IsOccupied)
                {
                    return false;
                }

                //all tiles must adhear to the tile selection mode
                var tileSelectionMode = TileFilterCriteria();
                if (!TileFilterHelper.FilterTile(t, tileSelectionMode))
                {
                    return false;
                }
                
                
            }

            //if all tiles are valid, return true
            return true;
        }

        private void Summon(CardController cardController, Action<bool> onComplete, Tile tile)
        {
            if (tile.IsOccupied)
            {
                onComplete?.Invoke(true); // graceful fail
                return;
            }

            var pawnFactory = ServiceLocator.Get<PawnFactory>();
            if (!pawnFactory)
            {
                Debug.LogError("SummonUnitPlay: PawnFactory not found.");
                onComplete?.Invoke(false);
                return;
            }

            var unit = Pawn;
            var pawnController = pawnFactory.CreatePawn(unit, tile);

            pawnController.AssignSummonCard(cardController);

            onComplete?.Invoke(true);
        }

        public override void BlindPlay(CardController cardController, Action<bool> onComplete)
        {
            var tilemap = ServiceLocator.Get<TilemapController>();
            var randTile = tilemap.AllTiles().Where(t => TileFilterHelper.FilterTile(t, TileFilterCriteria())).ToList()
                .SelectRandom();
            Summon(cardController, onComplete, randTile);
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