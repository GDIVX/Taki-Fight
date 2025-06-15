using System;
using System.Collections.Generic;
using Sirenix.Serialization;
using UnityEngine;

namespace Utilities
{
    [Serializable]
    public class Stat : Observable<int>
    {
        [OdinSerialize] [SerializeField] private int _baseValue;
        [OdinSerialize] [SerializeField] private Dictionary<object, int> _modifiers = new Dictionary<object, int>();

        public Stat(int baseValue = 0)
        {
            _baseValue = baseValue;
            UpdateValue();
        }

        public int BaseValue
        {
            get => _baseValue;
            set
            {
                if (_baseValue == value) return;
                _baseValue = value;
                UpdateValue();
            }
        }

        public void SetModifier(object source, int modifier)
        {
            _modifiers[source] = modifier;
            UpdateValue();
        }

        public void RemoveModifier(object source)
        {
            if (_modifiers.Remove(source))
                UpdateValue();
        }

        private void UpdateValue()
        {
            int totalModifier = 0;
            foreach (var mod in _modifiers.Values)
                totalModifier += mod;

            base.Value = _baseValue + totalModifier;
        }

        // Prevent external override of value
        public new int Value => base.Value;

        public void ForceRecalculate()
        {
            UpdateValue();
            ForceNotify();
        }
    }
}