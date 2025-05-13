using UnityEngine;

namespace Utilities
{
    public abstract class ScriptableService<T> : ScriptableObject where T : ScriptableObject
    {
        private void Awake()
        {
            ServiceLocator.Register(this as T);
        }
    }
}