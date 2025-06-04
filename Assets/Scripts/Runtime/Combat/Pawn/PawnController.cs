using System;
using System.Collections;
using System.Collections.Generic;
using CodeMonkey.HealthSystemCM;
using JetBrains.Annotations;
using Runtime.Combat.StatusEffects;
using Runtime.Combat.Tilemap;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using Utilities;

namespace Runtime.Combat.Pawn
{
    public class PawnController : MonoBehaviour
    {
        [SerializeField, Required] private PawnView _view;
        [SerializeField, Required] private StatusEffectHandler _statusEffectHandler;
        [SerializeField] private PawnCombat _combat;
        [SerializeField] private PawnTilemapHelper _tilemapHelper;
        [SerializeField] private PawnMovement _movement;

        public PawnOwner Owner { get; set; }

        public PawnData Data { get; private set; }
        [ShowInInspector] public HealthSystem Health { get; private set; }
        public bool IsAgile { get; private set; }
        public bool IsProcessingTurn { get; private set; }


        internal PawnCombat Combat => _combat;
        internal PawnTilemapHelper TilemapHelper => _tilemapHelper;
        internal PawnMovement Movement => _movement;
        public PawnView View => _view;
        public event Action OnKilled;

        public void Init(PawnData data)
        {
            if (data == null) throw new ArgumentNullException(nameof(data));

            // Components
            _combat = new PawnCombat(this, data);
            _tilemapHelper = new(data.Size, this);
            _movement = new(this, _tilemapHelper, data.Speed);

            Health = new HealthSystem(data.Health);
            Health.OnDead += OnDead;

            Combat.AttackRange = data.AttackRange;

            _statusEffectHandler ??= GetComponent<StatusEffectHandler>();
            _statusEffectHandler.Init(this);


            _view ??= GetComponent<PawnView>();
            _view.Init(this, Combat.Defense, data);

            IsAgile = data.IsAgile;
            Owner = data.Owner;

            gameObject.name = $"{data.name}_{Guid.NewGuid()}";

            Data = data;

            // Execute onSummon strategies
            ExecuteStrategies(data.OnSummonStrategies);
        }

        private void OnDead(object sender, EventArgs e)
        {
            Kill();
        }

        public void Kill()
        {
            ExecuteStrategies(Data.OnKilledStrategies);
            OnKilled?.Invoke();
            Remove(true);
        }

        public void Remove(bool animate)
        {
            if (animate)
                _view.OnDead(() => Destroy(gameObject));
            else
                Destroy(gameObject);

            _statusEffectHandler.Clear();
            var tilemap = ServiceLocator.Get<TilemapController>();
            tilemap?.RemoveUnit(this);
        }


        public void OnTurn()
        {
            if (IsProcessingTurn) return; // Prevent multiple turns at once
            if (Health.IsDead()) return; // Prevent dead units from taking turns

            IsProcessingTurn = true;
            _statusEffectHandler.Apply();
            _movement.ResetSpeed();

            // Execute onTurnStart strategies
            ExecuteStrategies(Data.OnTurnStartStrategies);

            StartCoroutine(ProcessTurn());
        }

        private IEnumerator ProcessTurn()
        {
            // Check if this unit can attack first
            while (true)
            {
                var target = Combat.ChooseTarget();
                if (target)
                {
                    StartCoroutine(Combat.Attack(target, () => IsProcessingTurn = false));
                    yield break;
                }

                var isWaitingForMovement = true;
                // If the unit has no available speed or cannot move, break the loop
                if (_movement.AvilableSpeed <= 0 || !Movement.TryToMove(() => { isWaitingForMovement = false; }))
                {
                    break;
                }

                yield return new WaitUntil(() => !isWaitingForMovement);
            }

            IsProcessingTurn = false;
        }

        private bool TrySetMultiTilePosition(Tile anchor)
        {
            if (anchor == null) return false;
            var tilemap = ServiceLocator.Get<TilemapController>();
            if (tilemap == null) return false;

            var size = TilemapHelper.Size;
            var newTiles = tilemap.GenerateFootprintUnbounded(anchor.Position, size);
            if (newTiles.Length != size.x * size.y) return false; // missing tiles

            foreach (var t in TilemapHelper.OccupiedTiles)
                t.SetPawn(null);

            TilemapHelper.OccupiedTiles = new List<Tile>(newTiles);
            TilemapHelper.AnchorTile = anchor;

            foreach (var t in TilemapHelper.OccupiedTiles)
                t.SetPawn(this);

            return true;
        }

        internal void SpawnAtPosition(Tile anchor)
        {
            if (!TrySetMultiTilePosition(anchor)) return;
            _view.SpawnAtPosition(anchor.Position);
        }

        internal void MoveToPosition(Tile tile, Action onComplete)
        {
            if (!TrySetMultiTilePosition(tile)) return;
            _view.MoveToPosition(tile.Position, onComplete);
        }

        public void ApplyStatusEffect([NotNull] StatusEffectData data, int stack)
        {
            if (!data)
            {
                Debug.LogError("StatusEffectData is null.");
                return;
            }

            var effect = data.CreateStatusEffect(stack);
            _statusEffectHandler.Add(effect, data.Icon, data.Tooltip);
        }

        public int GetStatusEffectStacks(Type type)
        {
            var effect = _statusEffectHandler.Get(type);
            return effect?.Stack.Value ?? 0;
        }

        public void ClearStatusEffects() => _statusEffectHandler.Clear();

        internal void ExecuteStrategies(List<PawnStrategyData> strategies)
        {
            if (strategies == null)
            {
                Debug.LogWarning("ExecuteStrategies: The strategies list is null.");
                return;
            }

            foreach (var strategyData in strategies)
            {
                if (strategyData.Strategy == null)
                {
                    Debug.LogWarning("ExecuteStrategies: A strategy in the list is null.");
                    continue;
                }

                strategyData.Strategy.Play(this, success =>
                {
                    if (!success)
                    {
                        Debug.LogWarning($"Strategy {strategyData.Strategy.name} failed.");
                    }
                    else
                    {
                        Debug.Log($"Strategy {strategyData.Strategy.name} succeeded.");
                    }
                });
            }
        }

        public void OverrideHealthSystem(HealthSystem health)
        {
            Health = health;
            Health.OnDead += OnDead;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _view.OnPointerEnter(eventData);
        }
    }
}