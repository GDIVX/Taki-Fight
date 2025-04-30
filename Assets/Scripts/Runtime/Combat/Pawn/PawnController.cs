using System;
using System.Collections;
using Runtime.Combat.Tilemap;
using CodeMonkey.HealthSystemCM;
using Runtime.Combat.StatusEffects;
using Runtime.Selection;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;
using Utilities;

namespace Runtime.Combat.Pawn
{
    public class PawnController : MonoBehaviour, ISelectableEntity, IPointerClickHandler
    {
        [SerializeField, Required] private PawnView _view;
        [SerializeField, Required] private StatusEffectHandler _statusEffectHandler;
        [ShowInInspector, ReadOnly] private Tile _tile;
        [ShowInInspector, ReadOnly] private PawnOwner _owner;

        public TrackedProperty<int> Defense;
        public TrackedProperty<int> Damage;
        public TrackedProperty<int> Attacks;
        public TrackedProperty<int> Speed;

        public PawnOwner Owner => _owner;
        public Tile Tile => _tile;
        [ShowInInspector, ReadOnly] public Vector2Int[] AttackRange { get; private set; }

        [ShowInInspector, ReadOnly] public bool IsFlyer { get; private set; }
        [ShowInInspector, ReadOnly] public Vector2Int MovementDirection { get; private set; }
        [ShowInInspector, ReadOnly] public int AvilableSpeed { get; private set; }

        public HealthSystem Health { get; private set; }
        public bool IsAgile { get; private set; }
        public bool IsProcessingTurn { get; private set; }
        public event Action<int, int> OnBeingAttacked;

        [Button]
        public PawnController Init(PawnData data)
        {
            if (data == null) throw new ArgumentNullException(nameof(data));

            AddHealth(data);
            AddStatusEffectHandler();

            Defense = new TrackedProperty<int>(data.Defense);
            Damage = new TrackedProperty<int>(data.Damage);
            Attacks = new TrackedProperty<int>(data.Attacks);
            Speed = new TrackedProperty<int>(data.Speed);

            IsFlyer = data.IsFlyer;
            MovementDirection = data.Direction;
            AttackRange = data.AttackRange;

            _view ??= GetComponent<PawnView>();
            _view.Init(this, Defense, data);

            SelectionService.Instance.OnSelectionComplete += _ => OnDeselected();
            SelectionService.Instance.Register(this);

            IsAgile = data.IsAgile;

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
            _view.OnDead(() => Destroy(gameObject));
        }

        private void OnValidate()
        {
            _view ??= gameObject.GetComponent<PawnView>();
        }

        [Button]
        public void ReceiveAttack(int damage)
        {
            if (damage <= 0)
            {
                OnBeingAttacked?.Invoke(damage, 0);
                return;
            }

            var finalDamage = CalculateDamage(damage);
            Health.Damage(finalDamage);
            ReduceDefense(damage);
            OnBeingAttacked?.Invoke(damage, finalDamage);
        }


        private int CalculateDamage(int attackPoints)
        {
            return Mathf.Max(0, attackPoints - Defense.Value);
        }

        private void ReduceDefense(int attackPoints)
        {
            Defense.Value = Mathf.Max(0, Defense.Value - attackPoints);
        }

        public void OnTurn()
        {
            IsProcessingTurn = true;

            // Apply status effects at the start of the turn
            _statusEffectHandler.Apply();

            // Initialize available speed for this turn
            AvilableSpeed = Speed.Value;

            // Process movement and attack logic
            while (AvilableSpeed > 0)
            {
                // Check if a hostile unit is in attack range
                if (IsHostileUnitInAttackRange(out var target))
                {
                    // Attack the target and end the turn
                    StartCoroutine(Attack(target, () => IsProcessingTurn = false));
                    return;
                }

                // Attempt to move
                if (!TryToMove())
                {
                    // If unable to move, end the turn
                    break;
                }
            }

            // End the turn
            IsProcessingTurn = false;
        }

        private bool TryToMove()
        {
            if (AvilableSpeed <= 0)
            {
                return false;
            }

            var nextTile = GetNextTile();

            // Stop if the next tile is invalid
            if (nextTile == Tile || nextTile == null)
            {
                return false;
            }

            // Handle flyer-specific movement logic
            if (IsFlyer)
            {
                return HandleFlyerMovement(nextTile);
            }

            // Stop if the next tile is occupied and the unit is not a flyer
            if (nextTile.IsOccupied)
            {
                return false;
            }

            // Stop if entering restricted territory
            if (Owner == PawnOwner.Player && nextTile.Owner == TileOwner.Enemy)
            {
                return false;
            }

            if (Owner == PawnOwner.Enemy && nextTile.Owner == TileOwner.castle)
            {
                return false;
            }

            // Move to the next tile
            MoveToPosition(nextTile);

            // Decrement available speed
            AvilableSpeed--;

            return true;
        }

        private Tile GetNextTile()
        {
            var tilemap = ServiceLocator.Get<TilemapController>();
            if (tilemap == null)
            {
                Debug.LogError("Tilemap not found");
                return Tile;
            }

            // Calculate the next tile position
            var nextPosition = _tile.Position + MovementDirection;

            // Get the next tile
            var nextTile = tilemap.GetTile(nextPosition);

            // Return null if the next tile is out of bounds
            if (nextTile == null)
            {
                Debug.LogWarning($"Next tile at position {nextPosition} is out of bounds.");
                return Tile;
            }

            return (Tile)nextTile; // Explicitly cast the nullable Tile to Tile
        }


        public bool IsHostileUnitInAttackRange(out PawnController pawn)
        {
            var tilemap = ServiceLocator.Get<TilemapController>();
            if (tilemap == null)
            {
                Debug.LogError("Tilemap not found");
                pawn = null;
                return false;
            }

            foreach (var offset in AttackRange)
            {
                var targetTile = tilemap.GetTile(_tile.Position + offset);

                // Ensure targetTile is not null before accessing its properties
                if (targetTile.HasValue && targetTile.Value.IsOccupied && targetTile.Value.Pawn.Owner != Owner)
                {
                    pawn = targetTile.Value.Pawn;
                    return true;
                }
            }

            pawn = null;
            return false;
        }


        private IEnumerator Attack(PawnController target, Action onComplete)
        {
            for (int i = 0; i < Attacks.Value; i++)
            {
                target.ReceiveAttack(Damage.Value);
                yield return null;
            }

            onComplete?.Invoke();
        }


        #region Status Effects

        [Button]
        public void ApplyStatusEffect(StatusEffectData statusEffectData, int stack)
        {
            var statusEffect = statusEffectData.CreateStatusEffect(stack);
            _statusEffectHandler.Add(statusEffect, statusEffectData.Icon, statusEffectData.Tooltip);
        }

        public int GetStatusEffectStacks(Type type)
        {
            var effect = _statusEffectHandler.Get(type);
            return effect?.Stack.Value ?? 0;
        }

        public void ClearStatusEffects()
        {
            _statusEffectHandler.Clear();
        }

        #endregion

        #region Selection

        public void TryToSelect()
        {
            if (SelectionService.Instance.CurrentState != SelectionState.InProgress)
            {
                return;
            }

            var predicate = SelectionService.Instance.Predicate;
            if (predicate.Invoke(this))
            {
                SelectionService.Instance.Select(this);
            }
        }

        public void OnSelected()
        {
            _view.OnSelected();
        }

        public void OnDeselected()
        {
            _view.ClearSelection();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
            {
                return;
            }

            if (SelectionService.Instance.CurrentState != SelectionState.InProgress)
            {
                return;
            }

            TryToSelect();
        }

        #endregion

        internal void SetPosition(Vector2Int position)
        {
            var arenaController = ServiceLocator.Get<TilemapController>();
            if (arenaController == null)
            {
                Debug.LogError("ArenaController not found");
                return;
            }
            var tile = arenaController.GetTile(position);
            SetPosition(tile);

        }

        private void SetPosition(Tile? tile)
        {
            if (!tile.HasValue)
            {
                Debug.LogError("Tile is null or out of bounds");
                return;
            }

            _tile.SetPawn(null);
            _tile = tile.Value; // Safely access the value of the nullable Tile
            _tile.SetPawn(this);
        }
        internal void SpawnAtPosition(Tile tile)
        {
            SetPosition(tile);

            // Call the view's spawn method
            _view.SpawnAtPosition(_tile.Position);
        }

        internal void MoveToPosition(Tile tile)
        {
            SetPosition(tile);

            // Call the view's move method
            _view.MoveToPosition(_tile.Position);
        }
        private bool HandleFlyerMovement(Tile? nextTile)
        {
            // Flyers skip occupied tiles but cannot end on them
            if (nextTile.HasValue && nextTile.Value.IsOccupied)
            {
                // Check if there is enough speed to move to the next tile and that the next tile is empty
                var tilemap = ServiceLocator.Get<TilemapController>();
                if (tilemap == null)
                {
                    Debug.LogError("Tilemap not found");
                    return false;
                }

                var nextPosition = nextTile.Value.Position + MovementDirection;
                var nextNextTile = tilemap.GetTile(nextPosition);

                if (AvilableSpeed <= 1 || !nextNextTile.HasValue || nextNextTile.Value.IsOccupied)
                {
                    return false; // Not enough speed or the next tile after is invalid
                }

                // Skip to the next tile
                AvilableSpeed--; // Deduct speed for skipping
                return TryToMove();
            }

            // Move to the next tile
            if (nextTile.HasValue)
            {
                MoveToPosition(nextTile.Value);
            }

            // Decrement available speed
            AvilableSpeed--;

            return true;
        }
    }
}