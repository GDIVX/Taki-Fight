﻿using Runtime.Combat.Pawn;
using Runtime.Events;
using UnityEngine;

namespace Runtime.CardGameplay.Card.CardBehaviour
{
    [CreateAssetMenu(fileName = "Attack Play", menuName = "Card/Strategy/Play/Attack", order = 0)]
    public class AttackTargetCardPlay : CardPlayStrategy
    {
        [SerializeField] private TargetingStrategy _targetingStrategy;


        public override void Play(PawnController caller, int potency)
        {
            HandleAttack(caller, potency);
        }

        protected virtual void HandleAttack(PawnController caller, int potency)
        {
            var target = _targetingStrategy.GetTarget();

            // It is possible that the target is dead or has no controller. Fire a warning to the log to be safe
            if (target == null)
            {
                Debug.LogError($"Card '{name}' tried to play on a target that has no controller.");
                return;
            }

            if (target.Health.IsDead())
            {
                Debug.LogWarning(
                    $"Card '{name}' tried to play on a target that is dead. This could indicate a normal gameplay situation.");
                return;
            }

            var finalDamage = potency + caller.AttackModifier.Value;
            target.ReceiveAttack(finalDamage);
        }
    }
}