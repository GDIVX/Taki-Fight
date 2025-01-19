using Runtime.Combat.Pawn;
using UnityEngine;

namespace Runtime.CardGameplay.Card.CardBehaviour
{
    [CreateAssetMenu(fileName = "Healing Strategy", menuName = "Card/Strategy/Play/Healing", order = 0)]
    public class HealingStrategy : CardPlayStrategy
    {
        public override void Play(PawnController caller, int potency)
        {
            caller.Health.Heal(potency);
        }
    }
}