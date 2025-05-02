using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Utilities;

namespace Runtime.Combat.Pawn
{
    [Serializable]
    internal class PawnCombat
    {
        public TrackedProperty<int> Defense;
        public TrackedProperty<int> Damage;
        public TrackedProperty<int> Attacks;

        [ShowInInspector, ReadOnly] public Vector2Int[] AttackRange { get; set; }

        public event Action<int, int> OnBeingAttacked;

        public PawnController Pawn { get; private set; }

        public PawnCombat(PawnController pawn, PawnData data)
        {
            Pawn = pawn;
            Defense = new(data.Defense);
            Damage = new(data.Damage);
            Attacks = new(data.Attacks);
        }

        public void ReceiveAttack(int damage)
        {
            if (damage <= 0)
            {
                OnBeingAttacked?.Invoke(damage, 0);
                return;
            }

            int finalDamage = Mathf.Max(0, damage - Defense.Value);
            Pawn.Health.Damage(finalDamage);
            Defense.Value = Mathf.Max(0, Defense.Value - damage);

            OnBeingAttacked?.Invoke(damage, finalDamage);
            Pawn.ExecuteStrategies(Pawn.Data.OnDamagedStrategies);
        }


        public IEnumerator Attack(PawnController target, Action onComplete)
        {
            for (int i = 0; i < Attacks.Value; i++)
            {
                target.Combat.ReceiveAttack(Damage.Value);
                yield return null;
            }
            onComplete?.Invoke();
            Pawn.ExecuteStrategies(Pawn.Data.OnAttackStrategies);
        }

        public bool IsHostileUnitInAttackRange(out PawnController pawn)
        {
            var tilemap = ServiceLocator.Get<TilemapController>();
            if (tilemap == null)
            {
                pawn = null;
                return false;
            }

            foreach (var offset in AttackRange)
            {
                var targetTile = tilemap.GetTile(Pawn.TilemapHelper.AnchorTile.Position + offset);
                if (targetTile == null || !targetTile.IsOccupied || targetTile.Pawn.Owner == Pawn.Owner)
                    continue;

                pawn = targetTile.Pawn;
                return true;
            }

            pawn = null;
            return false;
        }
    }
}
