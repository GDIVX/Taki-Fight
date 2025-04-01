using System;
using System.Collections.Generic;
using Runtime.Combat.Arena;
using Runtime.Combat.Pawn;
using Runtime.Selection;
using UnityEngine;

namespace Runtime.CardGameplay.Card.CardBehaviour
{
    [CreateAssetMenu(fileName = "Summon Unit", menuName = "Card/Strategy/Play/Summon Unit", order = 0)]
    public class SummonUnitPlay : CardPlayStrategy
    {
        [SerializeField] private List<PawnData> _units;
        [SerializeField] private LaneFilterHelper.LaneSelectionMode _laneSelectionMode;

        public override void Play(CardController cardController, int potency, Action<bool> onComplete)
        {
            //select lane

            SelectionService.Instance.RequestSelection
            (
                target => target is LaneSide laneSide && LaneFilterHelper.FilterLaneSide(laneSide, _laneSelectionMode),
                1,
                selectedEntities =>
                {
                    //summon unit
                    foreach (var selectableEntity in selectedEntities)
                    {
                        if (selectableEntity is not LaneSide laneSide) return;
                        foreach (var unit in _units)
                        {
                            if (laneSide.Count >= laneSide.PawnsLimit)
                            {
                                onComplete?.Invoke(true);
                                return;
                            }

                            laneSide.AddPawn(unit);
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