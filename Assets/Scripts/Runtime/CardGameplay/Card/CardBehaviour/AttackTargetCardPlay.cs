using Runtime.Combat.Pawn.Targeting;
using UnityEngine;

namespace Runtime.CardGameplay.Card.CardBehaviour
{
    [CreateAssetMenu(fileName = "Attack Play", menuName = "Card/Strategy/Play/Attack", order = 0)]
    public class AttackTargetCardPlay : CardPlayStrategy
    {
        [SerializeField] private int attackDamage;

        public override void Play(CardController card)
        {
            var target = PawnTargetingService.Instance.TargetedPawn;

            // It is possible that the target is dead or has no controller. Fire a warning to the log to be safe
            if (target == null)
            {
                Debug.LogError(
                    $"Card '{card}' tried to play, but no valid target was found. The target might be dead or missing.");
                return;
            }

            if (target.Controller == null)
            {
                Debug.LogError($"Card '{card}' tried to play on a target that has no controller. This is a bug.");
                return;
            }

            if (target.Controller.Health.IsDead())
            {
                Debug.LogWarning(
                    $"Card '{card}' tried to play on a target that is dead. This could indicate a normal gameplay situation.");
                return;
            }

            target.Controller.Attack(attackDamage);
        }
    }
}