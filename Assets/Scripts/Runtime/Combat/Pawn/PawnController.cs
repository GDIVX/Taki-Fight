using System;
using System.Collections;
using System.Collections.Generic;
using Runtime.Combat.Tilemap;
using CodeMonkey.HealthSystemCM;
using Runtime.Combat.StatusEffects;
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

        [ShowInInspector, ReadOnly] private List<Tile> _occupiedTiles = new();
        [ShowInInspector, ReadOnly] private Tile _anchorTile;
        [ShowInInspector, ReadOnly] private Vector2Int[] _footprint;

        public TrackedProperty<int> Defense;
        public TrackedProperty<int> Damage;
        public TrackedProperty<int> Attacks;
        public TrackedProperty<int> Speed;

        public PawnOwner Owner { get; private set; }
        public IReadOnlyList<Tile> OccupiedTiles => _occupiedTiles;
        public Tile AnchorTile => _anchorTile;

        public Vector2Int Size { get; private set; }
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

            Health = new HealthSystem(data.Health);
            Health.OnDead += OnDead;

            _statusEffectHandler ??= GetComponent<StatusEffectHandler>();
            _statusEffectHandler.Init(this);

            Defense = new(data.Defense);
            Damage = new(data.Damage);
            Attacks = new(data.Attacks);
            Speed = new(data.Speed);

            Size = data.Size;
            _footprint = GenerateFootprint(Size.x, Size.y);

            IsFlyer = data.IsFlyer;
            MovementDirection = data.Direction;
            AttackRange = data.AttackRange.ToArray();

            _view ??= GetComponent<PawnView>();
            _view.Init(this, Defense, data);

            IsAgile = data.IsAgile;
            Owner = data.Owner;

            gameObject.name = $"{data.name}_{Guid.NewGuid()}";

            return this;
        }

        private Vector2Int[] GenerateFootprint(int width, int height)
        {
            var list = new List<Vector2Int>();
            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                    list.Add(new Vector2Int(x, y));
            return list.ToArray();
        }

        public Vector2Int[] GetFootprintOffsets() => _footprint;

        private void OnDead(object sender, EventArgs e)
        {
            _view.OnDead(() => Destroy(gameObject));
        }

        [Button]
        public void ReceiveAttack(int damage)
        {
            if (damage <= 0)
            {
                OnBeingAttacked?.Invoke(damage, 0);
                return;
            }

            int finalDamage = Mathf.Max(0, damage - Defense.Value);
            Health.Damage(finalDamage);
            Defense.Value = Mathf.Max(0, Defense.Value - damage);

            OnBeingAttacked?.Invoke(damage, finalDamage);
        }

        public void OnTurn()
        {
            IsProcessingTurn = true;
            _statusEffectHandler.Apply();
            AvilableSpeed = Speed.Value;

            while (AvilableSpeed > 0)
            {
                if (IsHostileUnitInAttackRange(out var target))
                {
                    StartCoroutine(Attack(target, () => IsProcessingTurn = false));
                    return;
                }

                if (!TryToMove()) break;
            }

            IsProcessingTurn = false;
        }

        private bool TryToMove()
        {
            if (AvilableSpeed <= 0) return false;

            var nextTile = GetNextTile();

            if (nextTile == null)
            {
                return false;
            }

            if (nextTile == AnchorTile)
            {
                return false;
            }

            if (Owner == PawnOwner.Player && nextTile.Owner == TileOwner.Enemy) return false;
            if (Owner == PawnOwner.Enemy && nextTile.Owner == TileOwner.castle) return false;

            // Additional rule for pawns larger than 1x1
            if (Size.x > 1 || Size.y > 1)
            {
                var tilemap = ServiceLocator.Get<TilemapController>();
                if (tilemap != null)
                {
                    var footprint = tilemap.GenerateFootprintUnbounded(nextTile.Position, Size);
                    foreach (var tile in footprint)
                    {
                        if (tile.Owner == TileOwner.Enemy && Owner == PawnOwner.Player) return false;
                        if (tile.Owner == TileOwner.castle && Owner == PawnOwner.Enemy) return false;
                    }
                }
            }

            MoveToPosition(nextTile);
            AvilableSpeed--;

            Debug.Log($"Trying to move {gameObject.name} to {nextTile.Position}");

            return true;
        }

        private Tile GetNextTile()
        {

            var tilemap = ServiceLocator.Get<TilemapController>();
            if (tilemap == null) return _anchorTile;

            var nextAnchor = _anchorTile.Position + MovementDirection;
            var nextTile = tilemap.GetTile(nextAnchor);
            if (nextTile == null) return _anchorTile;

            var footprint = tilemap.GenerateFootprintUnbounded(nextAnchor, Size);
            if (footprint.Length != Size.x * Size.y) return _anchorTile;

            if (!IsFlyer)
            {
                foreach (var tile in footprint)
                {
                    if (tile.IsOccupied && tile.Pawn != this) return _anchorTile;
                }
            }

            return nextTile;
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
                var targetTile = tilemap.GetTile(_anchorTile.Position + offset);
                if (targetTile == null || !targetTile.IsOccupied || targetTile.Pawn.Owner == Owner)
                    continue;

                pawn = targetTile.Pawn;
                return true;
            }

            pawn = null;
            return false;
        }

        private bool TrySetMultiTilePosition(Tile anchor)
        {
            if (anchor == null) return false;
            var tilemap = ServiceLocator.Get<TilemapController>();
            if (tilemap == null) return false;

            var newTiles = tilemap.GenerateFootprintUnbounded(anchor.Position, Size);
            if (newTiles.Length != Size.x * Size.y) return false; // missing tiles

            if (!IsFlyer)
            {
                foreach (var tile in newTiles)
                {
                    if (tile.IsOccupied && tile.Pawn != this) return false;
                }
            }

            foreach (var t in _occupiedTiles)
                t.SetPawn(null);

            _occupiedTiles = new List<Tile>(newTiles);
            _anchorTile = anchor;

            foreach (var t in _occupiedTiles)
                t.SetPawn(this);

            return true;
        }


        internal void SpawnAtPosition(Tile anchor)
        {
            if (!TrySetMultiTilePosition(anchor)) return;
            _view.SpawnAtPosition(anchor.Position);
        }

        internal void MoveToPosition(Tile anchor)
        {
            if (!TrySetMultiTilePosition(anchor)) return;
            _view.MoveToPosition(anchor.Position);
        }

        public void ApplyStatusEffect(StatusEffectData data, int stack)
        {
            var effect = data.CreateStatusEffect(stack);
            _statusEffectHandler.Add(effect, data.Icon, data.Tooltip);
        }

        public int GetStatusEffectStacks(Type type)
        {
            var effect = _statusEffectHandler.Get(type);
            return effect?.Stack.Value ?? 0;
        }

        public void ClearStatusEffects() => _statusEffectHandler.Clear();

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left) return;
        }
    }
}
