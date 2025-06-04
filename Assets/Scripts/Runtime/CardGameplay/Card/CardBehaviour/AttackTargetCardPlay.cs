using System;
using Runtime.Combat.Pawn;
using Runtime.Combat.Tilemap;
using Runtime.Selection;
using UnityEngine;

namespace Runtime.CardGameplay.Card.CardBehaviour
{
    [CreateAssetMenu(fileName = "Attack Play", menuName = "Card/Strategy/Play/Attack", order = 0)]
    public class AttackTargetCardPlay : CardPlayStrategy
    {
        [SerializeField] private int _targetsCount;
        [SerializeField] private TileSelectionMode _tileSelectionMode = TileSelectionMode.EnemyOccupied;

        public override void Play(CardController cardController, int potency, Action<bool> onComplete)
        {
            SelectionService.Instance.RequestSelection(target =>
                    target is TileView tileView && TileFilterHelper.FilterTile(tileView.Tile, _tileSelectionMode),
                _targetsCount,
                selectedEntities =>
                {
                    if (selectedEntities.Count > 0)
                    {
                        selectedEntities.ForEach(entity =>
                        {
                            if (entity is not TileView tileView) return;
                            HandleAttack(tileView.Tile?.Pawn, potency);
                        });
                    }
                    else
                    {
                        Debug.LogWarning($"Card '{name}' selection was canceled or invalid.");
                    }

                    // Notify that play execution is complete (even if canceled)
                    onComplete?.Invoke(true);
                },
                () => onComplete?.Invoke(false)
                ,
                cardController.transform.position
            );
        }


        private void HandleAttack(PawnController target, int potency)
        {
            //TODO: Magic power stat
            if (target == null)
            {
                Debug.LogError($"Card '{name}' tried to attack a null target.");
                return;
            }

            if (target.Health.IsDead())
            {
                Debug.LogWarning($"Card '{name}' tried to attack a dead target. Might be expected.");
                return;
            }

            var finalDamage = potency;
            target.Combat.ReceiveAttack(finalDamage);
        }

        public override string GetDescription()
        {
            if (_targetsCount > 1) return "Deal {Potency} damage to " + _targetsCount + " targets.";

            return "Deal {Potency} damage.";
        }
    }
}