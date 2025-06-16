using Runtime.CardGameplay.Card.View;
using UnityEngine;

namespace Runtime.UI.Tooltip
{
    public static class KeywordLoader
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void InitializeKeywords()
        {
            var keywords = Resources.LoadAll<Keyword>(""); // Or use your own folder path
            foreach (var keyword in keywords)
            {
                // OnEnable should still be called, but you can register here directly to be extra safe.
                KeywordDictionary.Register(keyword);
                Debug.Log($"Registered keyword: {keyword.Header}");
            }
        }
    }
}