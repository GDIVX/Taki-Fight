using System;
using Sirenix.OdinInspector;
using UnityEngine;
using Utilities;

namespace Runtime.CardGameplay.Energy
{
    public class Energy : MonoService<Energy>
    {
        [SerializeField, BoxGroup("Settings")] private int _initialIncomePerTurn;

        [SerializeField] [Required] private EnergyView _view;

        [ShowInInspector, ReadOnly] private TrackedProperty<int> _currentAmount = new(0);
        [ShowInInspector, ReadOnly] private TrackedProperty<int> _incomePerTurn = new(0);

        public int Amount => _currentAmount.ReadOnlyValue;
        public int Income => _incomePerTurn.ReadOnlyValue;

        public void Reset()
        {
            Clear();
            _incomePerTurn.Value = _initialIncomePerTurn;
        }

        public event Action<int> OnAmountChanged
        {
            add => _currentAmount.OnValueChanged += value;
            remove => _currentAmount.OnValueChanged -= value;
        }

        public event Action<int> OnIncomeChanged
        {
            add => _incomePerTurn.OnValueChanged += value;
            remove => _incomePerTurn.OnValueChanged -= value;
        }

        public void Initialize()
        {
            _view.Initialize(this);
            Reset();
        }

        public void Add(int amount)
        {
            if (amount > 0)
                _currentAmount.Value += amount;
        }

        public void GainEnergyPerIncome()
        {
            _currentAmount.Value += _incomePerTurn.Value;
        }

        public void SetIncome(int newIncome)
        {
            _incomePerTurn.Value = newIncome;
        }

        public void Remove(int cost)
        {
            _currentAmount.Value = Mathf.Max(_currentAmount.Value - cost, 0);
        }

        public void Clear()
        {
            _currentAmount.Value = 0;
        }
    }
}