using System;
using System.Collections.Generic;
using Assets.Scripts.Runtime.Combat.Arena;
using Runtime.Combat.Arena;
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
        private TileSelectionMode _tileSelectionMode;

        public override void Play(CardController cardController, int potency, Action<bool> onComplete)
        {
            // Select tile
            SelectionService.Instance.RequestSelection
            (
                target => target is Tile tile && TileFilterHelper.FilterTile(tile, _tileSelectionMode),
                1,
                selectedEntities =>
                {
                    // Summon unit
                    foreach (var selectableEntity in selectedEntities)
                    {
                        if (selectableEntity is not Tile tile) return;

                        foreach (var unit in _units)
                        {
                            if (tile.IsOccupied)
                            {
                                onComplete?.Invoke(true);
                                return;
                            }

                            //Use the pawn factory to create the unit
                            var pawnFactory = ServiceLocator.Get<PawnFactory>();
                            var pawn = pawnFactory.CreatePawn(unit, tile);
                        }

                        onComplete?.Invoke(true);
                    }
                },
                () => onComplete?.Invoke(false),
                cardController.transform.position
            );
        }
    }
}
