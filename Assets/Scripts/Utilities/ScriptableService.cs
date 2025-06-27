using UnityEngine;

namespace Utilities
{
    public abstract class ScriptableService<T> : ScriptableObject where T : ScriptableObject
    {
        private T Self => this as T;

        protected virtual void OnEnable()
        {
            RegisterSelf();
        }

        protected virtual void OnDisable()
        {
            UnregisterSelf();
        }

        private void RegisterSelf()
        {
            if (Self && Application.isPlaying)
            {
                ServiceLocator.Register(Self);
                Debug.Log($"Registered {Self.name} as {typeof(T).Name}");
            }
        }

        private void UnregisterSelf()
        {
            if (Self) ServiceLocator.Unregister(Self);
        }
    }
}