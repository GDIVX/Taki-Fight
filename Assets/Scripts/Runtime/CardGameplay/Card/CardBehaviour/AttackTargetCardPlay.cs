﻿using Runtime.Combat.Pawn;
using Runtime.Combat.Pawn.Targeting;
using UnityEngine;

namespace Runtime.CardGameplay.Card.CardBehaviour
{
    [CreateAssetMenu(fileName = "Attack Play", menuName = "Card/Strategy/Play/Attack", order = 0)]
    public class AttackTargetCardPlay : CardPlayStrategy
    {
        [SerializeField] private int _attackDamage;
        [SerializeField] private TargetingStrategy _targetingStrategy;

        public override void Play(PawnController caller)
        {
            Play(caller, _attackDamage);
        }

        public override void Play(PawnController caller, int value)
        {
            var target = _targetingStrategy.GetTarget();

            // It is possible that the target is dead or has no controller. Fire a warning to the log to be safe
            if (target == null)
            {
                Debug.LogError(
                    $"Card '{name}' tried to play, but no valid target was found. The target might be dead or missing.");
                return;
            }

            if (target == null)
            {
                Debug.LogError($"Card '{name}' tried to play on a target that has no controller. This is a bug.");
                return;
            }

            if (target.Health.IsDead())
            {
                Debug.LogWarning(
                    $"Card '{name}' tried to play on a target that is dead. This could indicate a normal gameplay situation.");
                return;
            }

            var finalDamage = value + caller.Power;
            target.Attack(finalDamage);
        }
    }
}