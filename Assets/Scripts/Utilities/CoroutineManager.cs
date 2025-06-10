using System.Collections;
using UnityEngine;

namespace Utilities
{
    public class CoroutineRunner : MonoService<CoroutineRunner>
    {
        private static CoroutineRunner _instance;

        /// <summary>
        ///     Singleton-like access through ServiceLocator and auto-creation if necessary
        /// </summary>
        public static CoroutineRunner Instance
        {
            get
            {
                if (_instance == null)
                {
                    // Attempt to get the instance from ServiceLocator
                    _instance = ServiceLocator.Get<CoroutineRunner>();

                    if (_instance == null)
                    {
                        // Create a GameObject and attach CoroutineRunner if not found
                        var go = new GameObject("CoroutineRunner");
                        _instance = go.AddComponent<CoroutineRunner>();

                        // Optionally mark it to persist across scenes
                        DontDestroyOnLoad(go);
                    }
                }

                return _instance;
            }
        }

        private void Awake()
        {
            // Set instance and register with ServiceLocator
            if (!_instance)
            {
                _instance = this;
            }
            else if (_instance != this)
            {
                Debug.LogWarning("Multiple CoroutineRunner instances detected. Destroying duplicate.");
                Destroy(gameObject);
            }
        }

        /// <summary>
        ///     Starts a coroutine on the CoroutineRunner instance.
        /// </summary>
        /// <param name="coroutine">The IEnumerator coroutine to run.</param>
        public Coroutine RunCoroutine(IEnumerator coroutine)
        {
            return StartCoroutine(coroutine);
        }

        /// <summary>
        ///     Stops a running coroutine on the CoroutineRunner.
        /// </summary>
        /// <param name="coroutine">The Coroutine to stop.</param>
        public void StopRunningCoroutine(Coroutine coroutine)
        {
            if (coroutine != null) StopCoroutine(coroutine);
        }

        /// <summary>
        ///     Stops all currently running coroutines on this CoroutineRunner instance.
        /// </summary>
        public void StopAllRunningCoroutines()
        {
            StopAllCoroutines();
        }
    }
}
