using System;
using Runtime.Combat.Pawn;
using UnityEngine;

namespace Runtime.CardGameplay.Card.CardBehaviour
{
    [CreateAssetMenu(fileName = "Healing Strategy", menuName = "Card/Strategy/Play/Healing", order = 0)]
    public class HealingStrategy : CardPlayStrategy
    {
        public override void Play(PawnController caller, int potency, Action onComplete)
        {
            caller.Health.Heal(potency);
            onComplete?.Invoke();
        }
    }
}