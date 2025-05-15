using UnityEngine;
using UnityEngine.SceneManagement;

namespace Runtime.SceneManagementExtend
{
    public static class SceneManagerExtensions
    {
        private static AsyncOperation _activeLoadOperation;

        public static AsyncOperation GetActiveLoadOperation()
        {
            return _activeLoadOperation;
        }

        public static AsyncOperation SafeLoadSceneAsync(string sceneName, LoadSceneMode loadMode)
        {
            // If there is no active load operation, initiate a new one
            if (_activeLoadOperation != null) return _activeLoadOperation;

            _activeLoadOperation = SceneManager.LoadSceneAsync(sceneName, loadMode);
            if (_activeLoadOperation == null) return _activeLoadOperation;
            _activeLoadOperation.completed += operation => _activeLoadOperation = null;

            return _activeLoadOperation;
        }
    }
}