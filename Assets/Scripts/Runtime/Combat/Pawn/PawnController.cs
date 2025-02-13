using System;
using System.Collections.Generic;
using CodeMonkey.HealthSystemCM;
using JetBrains.Annotations;
using Runtime.Combat.Pawn.Targeting;
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


        public TrackedProperty<int> Defense;
        public TrackedProperty<int> DefenseModifier = new();
        public TrackedProperty<int> AttackModifier = new();
        public TrackedProperty<int> HealingModifier = new();

        public HealthSystem Health { get; private set; }


        /// <summary>
        /// Invoked when a pawn is attacked. First value is incoming attack points, second value is the damage taken
        /// </summary>
        public event Action<int, int> OnBeingAttacked;


        [Button]
        public PawnController Init([NotNull] PawnData data)
        {
            if (data == null) throw new ArgumentNullException(nameof(data));
            AddHealth(data);
            AddStatusEffectHandler();

            Defense = new TrackedProperty<int>
            {
                Value = data.Defense
            };

            _view ??= GetComponent<PawnView>();
            _view.Init(this, Defense, data);

            return this;
        }

        public PawnController AddTargeting(PawnTargetType targetType)
        {
            GetComponentInChildren<PawnTarget>().Init(this, targetType);
            return this;
        }

        private void AddStatusEffectHandler()
        {
            _statusEffectHandler ??= GetComponent<StatusEffectHandler>();
            _statusEffectHandler.Init(this);
        }

        private void AddHealth(PawnData data)
        {
            Health = new HealthSystem(data.Health);
            Health.OnDead += OnDead;
        }

        private void OnDead(object sender, EventArgs e)
        {
            _view.OnDead(() => { Destroy(gameObject); });
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
            _statusEffectHandler.Add(statusEffect, statusEffectData.Icon, statusEffectData.Tooltip);
        }


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

        public int GetStatusEffectStacks(Type type)
        {
            var effect = _statusEffectHandler.Get(type);
            if (effect != null)
            {
                return effect.Stack.Value;
            }

            return 0;
        }

        public void ClearStatusEffects()
        {
            _statusEffectHandler.Clear();
        }
    }
}