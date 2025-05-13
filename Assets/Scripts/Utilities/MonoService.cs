using UnityEngine;

namespace Utilities
{
    public abstract class MonoService<T> : MonoBehaviour where T : MonoBehaviour
    {
        private void Awake()
        {
            ServiceLocator.Register(this as T);
        }

        private void OnDestroy()
        {
            ServiceLocator.Unregister(this as T);
        }
    }
}