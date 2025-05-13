using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utilities;

namespace Runtime
{
    public class MainMenuManager : MonoBehaviour
    {
        public void OnNewRunClicked()
        {
            //unload the main menu
            SceneManager.UnloadSceneAsync("MainMenu");
            ServiceLocator.Get<GameManager>().StartRun();
        }

        public void OnExitClicked()
        {
#if UNITY_EDITOR
            EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}