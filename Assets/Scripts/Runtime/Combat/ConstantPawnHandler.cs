using System;
using CodeMonkey.HealthSystemCM;
using Runtime.Combat.Pawn;
using Runtime.Combat.Tilemap;
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

        public void CreatePawn(Vector2Int position)
        {
            if (_pawn) return;
            var tilemap = ServiceLocator.Get<TilemapController>();
            if (tilemap == null)
            {
                Debug.LogError("TilemapController is missing. Ensure TilemapController is properly initialized.");
                return;
            }

            var tile = tilemap.GetTile(position);
            if (tile == null)
            {
                Debug.LogWarning(
                    $"Tile not found at position {position}. Ensure TilemapController is correctly managing tiles.");
                return;
            }

            var factory = ServiceLocator.Get<PawnFactory>();
            if (!factory)
            {
                Debug.LogError("PawnFactory is missing. Ensure PawnFactory is properly registered.");
                return;
            }

            var pawn = factory.CreatePawn(_data, tile);
            if (!pawn)
            {
                Debug.LogError($"Failed to create pawn at position {position}. Check PawnFactory logic.");
                return;
            }

            // Set health
            pawn.Health.SetHealth(CurrentHealth.Value);
            // Track health changes
            pawn.Health.OnHealthChanged += OnHealthChanged;

            _pawn = pawn;
        }

        public void RemovePawn()
        {
            if (!_pawn) return;
            _pawn.Health.OnHealthChanged -= OnHealthChanged;
            _pawn.Remove(false);
            _pawn = null;
        }

        private void OnHealthChanged(object sender, EventArgs e)
        {
            if (sender is HealthSystem health) CurrentHealth.Value = health.GetHealth();
        }
    }
}