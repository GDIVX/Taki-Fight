using System;
using Runtime.Combat.Pawn;
using UnityEngine;

namespace Runtime.CardGameplay.Card.CardBehaviour
{
    [CreateAssetMenu(fileName = "Attack Play", menuName = "Card/Strategy/Play/Defend", order = 0)]
    public class DefenseCardPlay : CardPlayStrategy
    {
        public override void Play(PawnController caller, int potency, Action onComplete)
        {
            caller.Defense.Value += potency;
            onComplete?.Invoke();
        }
    }
}