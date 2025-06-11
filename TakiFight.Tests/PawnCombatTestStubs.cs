using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;

namespace Runtime.Combat.Pawn
{
    public class PawnData
    {
        public int Defense { get; set; }
        public int Damage { get; set; }
        public int Attacks { get; set; }
        public AttackFeedback.AttackFeedbackStrategy AttackFeedbackStrategy { get; set; }
        public List<PawnStrategyData> OnHitStrategies { get; } = new();
        public List<PawnStrategyData> OnAttackStrategies { get; } = new();
    }

    public struct PawnStrategyData
    {
    }

    public class PawnController : MonoBehaviour
    {
        public PawnData Data { get; private set; }
        public PawnCombat Combat { get; private set; }

        public void Init(PawnData data)
        {
            Data = data;
            Combat = new PawnCombat(this, data);
        }

        internal void ExecuteHitStrategies(List<PawnStrategyData> strategies, PawnController target, ref int damage)
        {
        }

        internal void ExecuteStrategies(List<PawnStrategyData> strategies)
        {
        }
    }

    public class PawnCombat
    {
        public Observable<int> Defense { get; }
        public Observable<int> Damage { get; }
        public Observable<int> Attacks { get; }
        public PawnController Pawn { get; }

        public PawnCombat(PawnController pawn, PawnData data)
        {
            Pawn = pawn;
            Defense = new Observable<int>(data.Defense);
            Damage = new Observable<int>(data.Damage);
            Attacks = new Observable<int>(data.Attacks);
        }

        public void ReceiveAttack(int damage)
        {
        }

        public IEnumerator Attack(PawnController target, Action onComplete)
        {
            Pawn.Data.AttackFeedbackStrategy?.Play(Pawn, target, null);

            for (int i = 0; i < Attacks.Value; i++)
            {
                int attackDamage = Damage.Value;
                Pawn.ExecuteHitStrategies(Pawn.Data.OnHitStrategies, target, ref attackDamage);
                target.Combat.ReceiveAttack(attackDamage);
                yield return null;
            }

            onComplete?.Invoke();
            Pawn.ExecuteStrategies(Pawn.Data.OnAttackStrategies);
        }
    }
}
