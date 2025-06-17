using System;
using System.Linq;
using Runtime.CardGameplay.Card.CardBehaviour.Predicates;
using Runtime.Combat.Tilemap;
using Runtime.Selection;
using UnityEngine;
using Utilities;

namespace Runtime.CardGameplay.Card.CardBehaviour
{
    public abstract class AOECardPlayStrategy : CardPlayStrategy
    {
        protected AOEPlayParams aoeParams;

        protected abstract void ApplyEffect(Tile tile);

        public override void Play(CardController cardController, Action<bool> onComplete)
        {
            var tilemap = ServiceLocator.Get<TilemapController>();
            if (tilemap == null)
            {
                Debug.LogError($"{GetType().Name}: TilemapController not found.");
                onComplete?.Invoke(false);
                return;
            }

            SelectionService.Instance.SearchSize = aoeParams.AreaSize;
            SelectionService.Instance.RequestSelection(
                target =>
                {
                    if (target is not TileView tileView)
                    {
                        return false;
                    }

                    return TileFilterHelper.FilterTile(tileView.Tile, aoeParams.TileFilter);
                },
                1,
                selectedEntities =>
                {
                    if (selectedEntities.Count == 0 || selectedEntities[0] is not TileView tileView)
                    {
                        onComplete?.Invoke(false);
                        return;
                    }

                    var tiles = tilemap.GenerateFootprintUnbounded(tileView.Tile.Position, aoeParams.AreaSize);
                    foreach (var tile in tiles)
                    {
                        if (tile == null)
                        {
                            continue;
                        }

                        if (!TileFilterHelper.FilterTile(tile, aoeParams.TileFilter))
                        {
                            continue;
                        }

                        ApplyEffect(tile);
                    }

                    onComplete?.Invoke(true);
                },
                () => onComplete?.Invoke(false),
                cardController.transform.position);
        }

        public override void BlindPlay(CardController cardController, Action<bool> onComplete)
        {
            var tilemap = ServiceLocator.Get<TilemapController>();
            var tile = tilemap.AllTiles().Where(t => TileFilterHelper.FilterTile(t, aoeParams.TileFilter)).ToList()
                .SelectRandom();

            ApplyEffect(tile);
        }

        public override void Initialize(PlayStrategyData playStrategyData, CardController cardController)
        {
            aoeParams = playStrategyData.Parameters as AOEPlayParams;
            base.Initialize(playStrategyData, cardController);
        }
    }
}