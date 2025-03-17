using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Utilities
{
    [Serializable]
    public class TrackedProperty<T>
    {
        public event Action<T> OnValueChanged;
        [SerializeField] private T _value;

        public TrackedProperty(T initialValue)
        {
            _value = initialValue;
        }

        [Button]
        public T Value
        {
            get => _value;
            set
            {
                this._value = value;
                OnValueChanged?.Invoke(Value);
            }
        }

        public T ReadOnlyValue => Value;
    }
}