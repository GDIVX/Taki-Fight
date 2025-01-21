using System;
using Runtime.Combat.Pawn;
using UnityEngine;

namespace Runtime.CardGameplay.Card.CardBehaviour.Feedback
{
    public abstract class FeedbackStrategy : ScriptableObject
    {
        public abstract void Animate(PawnController caller, Action onComplete);
    }
}