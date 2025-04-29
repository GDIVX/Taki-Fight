using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Runtime.Combat.Tilemap;
using CodeMonkey.HealthSystemCM;
using Runtime.Combat.Tilemap;
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

            _statusEffectHandler.Apply();

            //TODO: Refactor attack to work with the new tile system

            ////If this can attack, find the first pawn on the other side and attack it
            //if (Attacks.Value <= 0 || Damage.Value <= 0)
            //{
            //    IsProcessingTurn = false;
            //    return;
            //}

            ////find an opponent and attack
            //var opponent = _side.Other().First();
            //if (!opponent)
            //{
            //    IsProcessingTurn = false;
            //    return;
            //}

            //StartCoroutine(Attack(opponent, () => IsProcessingTurn = false));
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

        internal void SetPosition(Tile tile)
        {
            _tile = tile;
            _view.SetPosition(_tile.Position);
            _tile.SetPawn(this);
        }
    }
}