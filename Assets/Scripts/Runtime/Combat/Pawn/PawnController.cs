using System;
using CodeMonkey.HealthSystemCM;
using Runtime.Combat.StatusEffects;
using Runtime.Selection;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using Utilities;

namespace Runtime.Combat.Pawn
{
    public class PawnController : MonoBehaviour, ISelectableEntity, IPointerClickHandler
    {
        [SerializeField, Required] private PawnView _view;
        [SerializeField, Required] private StatusEffectHandler _statusEffectHandler;

        public TrackedProperty<int> Defense;
        public TrackedProperty<int> Damage = new(0);
        public TrackedProperty<int> Attacks = new(0);

        public HealthSystem Health { get; private set; }

        public event Action<int, int> OnBeingAttacked;

        [Button]
        public PawnController Init(PawnData data)
        {
            if (data == null) throw new ArgumentNullException(nameof(data));

            AddHealth(data);
            AddStatusEffectHandler();

            Defense = new TrackedProperty<int>(data.Defense);

            _view ??= GetComponent<PawnView>();
            _view.Init(this, Defense, data);

            SelectionService.Instance.OnSelectionComplete += _ => OnDeselected();
            SelectionService.Instance.Register(this);

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

        [Button]
        public void ApplyStatusEffect(StatusEffectData statusEffectData, int stack)
        {
            var statusEffect = statusEffectData.CreateStatusEffect(stack);
            _statusEffectHandler.Add(statusEffect, statusEffectData.Icon, statusEffectData.Tooltip);
        }

        private int CalculateDamage(int attackPoints)
        {
            return Mathf.Max(0, attackPoints - Defense.Value);
        }

        private void ReduceDefense(int attackPoints)
        {
            Defense.Value = Mathf.Max(0, Defense.Value - attackPoints);
        }

        public void OnTurnStart()
        {
            _statusEffectHandler.OnTurnStart();
        }

        public void OnTurnEnd()
        {
            _statusEffectHandler.OnTurnEnd();
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

        // =======================================
        //  SELECTION SERVICE INTEGRATION
        // =======================================

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
    }
}