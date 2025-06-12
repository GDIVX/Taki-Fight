using System;
using Runtime.Combat.Pawn;
using Runtime.Combat.Tilemap;
using Runtime.Selection;
using UnityEngine;

namespace Runtime.CardGameplay.Card.CardBehaviour
{
    [CreateAssetMenu(fileName = "Capture Play", menuName = "Card/Strategy/Play/Capture", order = 0)]
    public class CapturePawnPlay : CardPlayStrategy
    {
        private GetTilesParams _params;

        public override void Play(CardController cardController, Action<CardPlayResult> onComplete)
        {
            SelectionService.Instance.RequestSelection(
                target => target is TileView tileView && TileFilterHelper.FilterTile(tileView.Tile, _params.TileFilter) && tileView.Tile.IsOccupied,
                _params.TargetsCount,
                selectedEntities =>
                {
                    var success = true;
                    foreach (var entity in selectedEntities)
                    {
                        if (entity is not TileView tileView) continue;
                        var pawn = tileView.Tile.Pawn;
                        if (pawn != null && !pawn.Capture(Potency))
                        {
                            success = false;
                        }
                    }
                    onComplete?.Invoke(new CardPlayResult(success));
                },
                () => onComplete?.Invoke(new CardPlayResult(false)),
                cardController.transform.position);
        }

        public override string GetDescription()
        {
            return _params.TargetsCount > 1
                ? $"Capture {_params.TargetsCount} enemies"
                : "Capture an enemy";
        }

        public override void Initialize(PlayStrategyData playStrategyData)
        {
            _params = playStrategyData.Parameters as GetTilesParams;
            base.Initialize(playStrategyData);
        }
    }
}
