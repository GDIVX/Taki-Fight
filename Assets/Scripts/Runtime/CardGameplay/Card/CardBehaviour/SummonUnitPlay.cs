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
        [SerializeField] private List<PawnData> _units;
        [SerializeField] private TileSelectionMode _tileSelectionMode;

        public override void Play(CardController cardController, int potency, Action<bool> onComplete)
        {
            SelectionService.Instance.RequestSelection
            (
                target => target is TileView tv && TileFilterHelper.FilterTile(tv.Tile, _tileSelectionMode),
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

                    var unit = _units[0]; // pick first or roll/random
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


