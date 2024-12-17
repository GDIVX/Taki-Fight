using System;
using System.Collections.Generic;
using CodeMonkey.HealthSystemCM;
using JetBrains.Annotations;
using Runtime.Combat.StatusEffects;
using Sirenix.OdinInspector;
using UnityEngine;
using Utilities;

namespace Runtime.Combat.Pawn
{
    public class PawnController : MonoBehaviour
    {
        [SerializeField, Required] private PawnView _view;
        [SerializeField, Required] private StatusEffectHandler _statusEffectHandler;
        [SerializeField] protected float WaitBeforeDestroyingObjectOnDeath;


        //TODO: handle bonuses via buffs
        public TrackedProperty<int> Defense;
        public TrackedProperty<int> DefenseModifier;
        public TrackedProperty<int> AttackModifier;

        public HealthSystem Health { get; private set; }


        /// <summary>
        /// Invoked when a pawn is attacked. First value is incoming attack points, second value is the damage taken
        /// </summary>
        public event Action<int, int> OnBeingAttacked;


        [Button]
        public void Init([NotNull] PawnData data)
        {
            if (data == null) throw new ArgumentNullException(nameof(data));
            Health = new HealthSystem(data.Health);
            _statusEffectHandler ??= GetComponent<StatusEffectHandler>();
            _statusEffectHandler.Init(this);

            Defense = new TrackedProperty<int>
            {
                Value = data.Defense
            };
            _view.Init(this, Defense, data);
        }

        private void OnValidate()
        {
            _view ??= gameObject.GetComponent<PawnView>();
        }

        [Button]
        public void ReceiveAttack(int attackPoints)
        {
            if (attackPoints <= 0)
            {
                //We still want to call damage to invoke UI updates
                Health.Damage(0);
                OnBeingAttacked?.Invoke(attackPoints, 0);
                return;
            }

            var finalDamage = CalculateDamage(attackPoints);

            Health.Damage(finalDamage);

            ReduceDefense(attackPoints);
            OnBeingAttacked?.Invoke(attackPoints, finalDamage);
        }

        [Button]
        public void ApplyStatusEffect(StatusEffectData statusEffectData, int stack)
        {
            var statusEffect = statusEffectData.CreateStatusEffect(stack);
            _statusEffectHandler.Add(statusEffect);
        }

        //TODO:
        // [Button]
        // public void RemoveStatusEffect<T>() where T : IStatusEffect
        // {
        //     
        // }

        private int CalculateDamage(int attackPoints)
        {
            return Mathf.Max(0, attackPoints - Defense.Value);
        }

        private void ReduceDefense(int attackPoints)
        {
            Defense.Value = Mathf.Max(0, Defense.Value - attackPoints);
        }

        public void OnTurnStart()
        {
            _statusEffectHandler.OnTurnStart();
        }

        public void OnTurnEnd()
        {
            _statusEffectHandler.OnTurnEnd();
        }
    }
}