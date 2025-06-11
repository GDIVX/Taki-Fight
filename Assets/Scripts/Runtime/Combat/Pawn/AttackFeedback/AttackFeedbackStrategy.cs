using System;
using UnityEngine;

namespace Runtime.Combat.Pawn.AttackFeedback
{
    public abstract class AttackFeedbackStrategy : ScriptableObject
    {
        public virtual void Initialize()
        {
        }

        public abstract void Play(PawnController attacker, PawnController target, Action onComplete);
    }
}
