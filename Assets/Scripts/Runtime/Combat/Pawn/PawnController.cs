using CodeMonkey.HealthSystemCM;
using Sirenix.OdinInspector;
using UnityEngine;
using Utilities;

namespace Runtime.Combat.Pawn
{
    public class PawnController : MonoBehaviour
    {
        [SerializeField, Required] private PawnView view;
        private int _defensePoints;

        public TrackedProperty<int> defense;

        public HealthSystem Health { get; private set; }
        
        //TODO: handle bonuses via buffs
        
        /// <summary>
        /// A flat bonus to attack damage
        /// </summary>
        public int Power { get; set; }
        

        [Button]
        public void Init(PawnData data)
        {
            Health = new HealthSystem(data.Health);

            defense = new()
            {
                Value = data.Defense
            };
            view.Init(Health, defense, data);
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
            return Mathf.Max(0, attackPoints - defense.Value);
        }

        private void ReduceDefense(int attackPoints)
        {
            defense.Value = Mathf.Max(0, defense.Value - attackPoints);
        }

    }
}