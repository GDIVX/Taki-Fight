using System;
using Runtime.Combat.Pawn;
using UnityEngine;

namespace Runtime.Combat.Pawn.AttackFeedback
{
    public struct AttackFeedbackStrategyData
    {
        public AttackFeedbackStrategy Strategy;
        public StrategyParams Parameters;
    }

    public abstract class AttackFeedbackStrategy : ScriptableObject
    {
        public virtual void Initialize(AttackFeedbackStrategyData data)
        {
        }

        public abstract void Play(PawnController attacker, PawnController target, Action onComplete);
    }

    public class TestAttackFeedbackStrategy : AttackFeedbackStrategy
    {
        public bool Played { get; private set; }

        public override void Play(PawnController attacker, PawnController target, Action onComplete)
        {
            Played = true;
            onComplete?.Invoke();
        }
    }
}
