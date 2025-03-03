using System;
using Runtime.Combat.Pawn;
using Runtime.Selection;
using UnityEngine;

namespace Runtime.CardGameplay.Card.CardBehaviour
{
    [CreateAssetMenu(fileName = "Attack Play", menuName = "Card/Strategy/Play/Attack", order = 0)]
    public class AttackTargetCardPlay : CardPlayStrategy
    {
        public override void Play(CardController caller, int potency, Action<bool> onComplete)
        {
            SelectionService.Instance.RequestSelection(
                target => target is PawnController, // Ensure we select a valid PawnController
                1, // Expect exactly one selection
                selectedEntities =>
                {
                    if (selectedEntities.Count > 0 && selectedEntities[0] is PawnController target)
                    {
                        HandleAttack(target, potency);
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
                caller.transform.position
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
            target.ReceiveAttack(finalDamage);
        }
    }
}