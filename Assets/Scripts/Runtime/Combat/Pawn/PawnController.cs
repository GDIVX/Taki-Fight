using System;
using System.Collections;
using System.Collections.Generic;
using CodeMonkey.HealthSystemCM;
using JetBrains.Annotations;
using Sirenix.OdinInspector;
using UnityEngine;
using Utilities;

namespace Runtime.Combat.Pawn
{
    public class PawnController : MonoBehaviour
    {
        [SerializeField, Required] private PawnView _view;
        private int _defensePoints;

        public TrackedProperty<int> Defense;

        public HealthSystem Health { get; private set; }

        //TODO: handle bonuses via buffs

        /// <summary>
        /// A flat bonus to attack damage
        /// </summary>
        public int Power { get; set; }


        [Button]
        public void Init([NotNull] PawnData data)
        {
            if (data == null) throw new ArgumentNullException(nameof(data));
            Health = new HealthSystem(data.Health);

            Defense = new TrackedProperty<int>
            {
                Value = data.Defense
            };
            _view.Init(Health, Defense, data);
        }

        private void OnValidate()
        {
            _view ??= gameObject.GetComponent<PawnView>();
        }

        [Button]
        public void Attack(int attackPoints)
        {
            if (attackPoints <= 0)
            {
                //We still want to call damage to invoke UI updates
                Health.Damage(0);
                return;
            }

            var finalDamage = CalculateDamage(attackPoints);

            Health.Damage(finalDamage);

            ReduceDefense(attackPoints);
        }

        private int CalculateDamage(int attackPoints)
        {
            return Mathf.Max(0, attackPoints - Defense.Value);
        }

        private void ReduceDefense(int attackPoints)
        {
            Defense.Value = Mathf.Max(0, Defense.Value - attackPoints);
        }
    }
}