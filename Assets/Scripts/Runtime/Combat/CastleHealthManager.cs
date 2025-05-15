using CodeMonkey.HealthSystemCM;
using Runtime.Combat.Pawn;
using Runtime.Combat.Tilemap;
using Runtime.Events;
using Sirenix.OdinInspector;
using UnityEngine;
using Utilities;

namespace Runtime.Combat
{
    /// <summary>
    ///     This class help with creating and tracking pawns that have consistent health between battles
    /// </summary>
    public class CastleHealthManager : MonoService<CastleHealthManager>, IGetHealthSystem
    {
        [ShowInInspector] private PawnData _data;
        [ShowInInspector] private PawnController _pawn;

        [ShowInInspector] public HealthSystem Health { get; private set; }


        public void Reset()
        {
            Health.SetHealth(Health.GetHealthMax());
        }


        public HealthSystem GetHealthSystem()
        {
            return Health;
        }

        public CastleHealthManager Init(PawnData data)
        {
            _data = data;
            Health = new HealthSystem(data.Health);
            return this;
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
            pawn.OverrideHealthSystem(Health);

            _pawn = pawn;

            NotifyCastlePawnCreated(pawn);
        }

        private void NotifyCastlePawnCreated(PawnController pawn)
        {
            var initializedEvent = new CastlePawnInitializedEvent(Health, pawn);
            ServiceLocator.Get<EventBus>().Publish(initializedEvent);
        }

        public void RemovePawn()
        {
            if (!_pawn) return;
            _pawn.Remove(false);
            _pawn = null;
        }

        public class CastlePawnInitializedEvent
        {
            public readonly HealthSystem HealthSystem;
            public readonly PawnController Pawn;

            public CastlePawnInitializedEvent(HealthSystem healthSystem, PawnController pawn)
            {
                HealthSystem = healthSystem;
                Pawn = pawn;
            }
        }
    }
}