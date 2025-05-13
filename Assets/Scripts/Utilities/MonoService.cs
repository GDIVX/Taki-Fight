using UnityEngine;

namespace Utilities
{
    public abstract class MonoService : MonoBehaviour
    {
        private void Awake()
        {
            ServiceLocator.Register(this);
        }
    }
}