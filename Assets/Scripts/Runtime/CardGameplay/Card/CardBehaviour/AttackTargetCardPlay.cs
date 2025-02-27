using System;
using Runtime.Combat.Pawn;
using Runtime.Selection;
using UnityEngine;

namespace Runtime.CardGameplay.Card.CardBehaviour
{
    [CreateAssetMenu(fileName = "Attack Play", menuName = "Card/Strategy/Play/Attack", order = 0)]
    public class AttackTargetCardPlay : CardPlayStrategy
    {
        public override void Play(PawnController caller, int potency, Action onComplete)
        {
            SelectionService.Instance.RequestSelection(
                target => target is PawnController, // Ensure we select a valid PawnController
                1, // Expect exactly one selection
                selectedEntities =>
                {
                    if (selectedEntities.Count > 0 && selectedEntities[0] is PawnController target)
                    {
                        HandleAttack(caller, target, potency);
                    }
                    else
                    {
                        Debug.LogWarning($"Card '{name}' selection was canceled or invalid.");
                    }

                    // Notify that play execution is complete (even if canceled)
                    onComplete?.Invoke();
                }
            );
        }

        private void HandleAttack(PawnController caller, PawnController target, int potency)
        {
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

            var finalDamage = potency + caller.AttackModifier.Value;
            target.ReceiveAttack(finalDamage);
        }
    }
}