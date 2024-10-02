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

        public TrackedProperty<int> Defense;

        private HealthSystem Health { get; set; }

        public void Init(PawnData data)
        {
            Health = new HealthSystem(data.Health);

            Defense = new()
            {
                Value = data.Defense
            };
            view.Init(Health, Defense, data);
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