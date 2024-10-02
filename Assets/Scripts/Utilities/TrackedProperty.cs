using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Utilities
{
    [Serializable]
    public class TrackedProperty<T>
    {
        public event Action<T> OnValueChanged;
        [SerializeField] private T value;

        [Button]
        public T Value
        {
            get => value;
            set
            {
                this.value = value;
                OnValueChanged?.Invoke(Value);
            }
        }
    }
}