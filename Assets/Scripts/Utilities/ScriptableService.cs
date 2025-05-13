using UnityEngine;

namespace Utilities
{
    public abstract class ScriptableService : ScriptableObject
    {
        private void Awake()
        {
            ServiceLocator.Register(this);
        }
    }
}