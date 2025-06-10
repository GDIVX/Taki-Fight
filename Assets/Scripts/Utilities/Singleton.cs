using UnityEngine;

namespace Utilities
{
    public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T _instance;
        private static object _lock = new object();
        private static bool _applicationIsQuitting = false;

        public static T Instance
        {
            get
            {
                if (_applicationIsQuitting)
                {
                    Debug.LogWarning("[Singleton] Instance '" + typeof(T) +
                                     "' already destroyed. Returning null.");
                    return null;
                }

                lock (_lock)
                {
                    if (_instance != null) return _instance;
                    _instance = (T)FindFirstObjectByType(typeof(T));


                    if (_instance == null)
                    {
                        GameObject singleton = new GameObject();
                        _instance = singleton.AddComponent<T>();
                        singleton.name = typeof(T).ToString() + " (Singleton)";

                        DontDestroyOnLoad(singleton);

                        Debug.Log("[Singleton] An instance of " + typeof(T) +
                                  " is needed in the scene, so '" + singleton +
                                  "' was created with DontDestroyOnLoad.");
                    }
                    else
                    {
                        Debug.Log("[Singleton] Using instance already created: " +
                                  _instance.gameObject.name);
                    }

                    return _instance;
                }
            }
        }

        private void Awake()
        {
            //ensure that there is only one instance of this class
            if (_instance && _instance != this)
            {
                Debug.LogWarning(
                    $"[Singleton] Instance of '{typeof(T)}' already exists in scene. Destroying duplicate.");
                Destroy(gameObject);
            }
            else
            {
                _instance = this as T;
            }
        }

        public void OnDestroy()
        {
            _applicationIsQuitting = true;
        }
    }
}
