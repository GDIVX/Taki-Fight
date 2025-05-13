using System;
using CodeMonkey.HealthSystemCM;
using Runtime.Combat.Pawn;
using UnityEngine;
using Utilities;

namespace Runtime.Combat
{
    /// <summary>
    ///     This class help with creating and tracking pawns that have consistent health between battles
    /// </summary>
    public class ConstantPawnHandler
    {
        private readonly PawnData _data;
        private PawnController _pawn;

        public ConstantPawnHandler(PawnData data)
        {
            _data = data;
            MaxHealth = new TrackedProperty<float>(data.Health);
            CurrentHealth = new TrackedProperty<float>(data.Health);
        }

        public TrackedProperty<float> MaxHealth { get; }
        public TrackedProperty<float> CurrentHealth { get; }

        public void Heal(float amount)
        {
            CurrentHealth.Value = Mathf.Clamp(CurrentHealth.Value + amount, 0, MaxHealth.Value);
        }

        public void Damage(float amount)
        {
            CurrentHealth.Value = Mathf.Clamp(CurrentHealth.Value - amount, 0, MaxHealth.Value);
        }

        public void Reset()
        {
            CurrentHealth.Value = MaxHealth.ReadOnlyValue;
        }

        public PawnController CreatePawn(Vector2Int position)
        {
            var tilemap = ServiceLocator.Get<TilemapController>();
            var tile = tilemap.GetTile(position);

            var factory = ServiceLocator.Get<PawnFactory>();
            var pawn = factory.CreatePawn(_data, tile);

            //set health
            pawn.Health.SetHealth(CurrentHealth.Value);
            //track change to health
            pawn.Health.OnHealthChanged += OnHealthChanged;

            _pawn = pawn;

            return pawn;
        }

        public void RemovePawn()
        {
            if (!_pawn) return;
            _pawn.Health.OnHealthChanged -= OnHealthChanged;
            _pawn.Remove();
            _pawn = null;
        }

        private void OnHealthChanged(object sender, EventArgs e)
        {
            if (sender is HealthSystem health) CurrentHealth.Value = health.GetHealth();
        }
    }
}