using System;
using Sirenix.Serialization;
using UnityEngine;

namespace Utilities
{
    [Serializable]
    public class Observable<T>
    {
        [OdinSerialize] [SerializeField] private T _currentValue;

        public Observable(T initialValue = default)
        {
            _currentValue = initialValue;
        }

        public T Value
        {
            get => _currentValue;
            set
            {
                if (HasValueChanged(value)) return;
                _currentValue = value;
                OnValueChanged?.Invoke(_currentValue);
            }
        }

        public event Action<T> OnValueChanged;

        public void ForceNotify()
        {
            OnValueChanged?.Invoke(_currentValue);
        }

        public static implicit operator T(Observable<T> observable)
        {
            return observable._currentValue;
        }

        private bool HasValueChanged(T newValue)
        {
            return Equals(_currentValue, newValue);
        }
    }
}