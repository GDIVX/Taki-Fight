using System;

namespace Utilities
{
    public class TrackedProperty<T>
    {
        public event Action<T> OnValueChanged;
        private T _value;

        public T Value
        {
            get => _value;
            set
            {
                _value = value;
                OnValueChanged?.Invoke(Value);
            }
        }
    }
}