using Runtime.Combat.Pawn;
using UnityEngine;

namespace Runtime.CardGameplay.Card.CardBehaviour
{
    [CreateAssetMenu(fileName = "Overheal Strategy", menuName = "Card/Strategy/Play/Overheal", order = 0)]
    public class OverhealStrategy : CardPlayStrategy
    {
        [SerializeField] private CardPlayStrategy _excessHealStrategy;

        public override void Play(PawnController caller, int potency)
        {
            var preHealingHealth = caller.Health.GetHealth();
            var maxHealth = caller.Health.GetHealthMax();
            var healingNeeded = maxHealth - preHealingHealth;
            var excessHealing = potency - healingNeeded;

            caller.Health.Heal(healingNeeded);

            if (excessHealing > 0)
            {
                _excessHealStrategy.Play(caller, Mathf.FloorToInt(excessHealing));
            }
        }
    }
}