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
            view.InitHealth(Health);

            Defense = new()
            {
                Value = data.Defense
            };
        }

        [Button]
        public void Attack(int attackPoints)
        {
            var finalDamage = Mathf.Max(0, attackPoints - Defense.Value);
            Health.Damage(finalDamage);
        }
    }
}