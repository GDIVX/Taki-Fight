using Sirenix.OdinInspector;
using UnityEngine;

namespace Editor.ArtPipeline
{
    [CreateAssetMenu(fileName = "SpritePipelineSettings", menuName = "Pipeline/Sprite Pipeline Settings", order = 0)]
    public class SpritePipelineSettings : ScriptableObject
    {
        private static SpritePipelineSettings _instance;

        [Header("Prefix Settings for Sprite Types")]
        [Tooltip("Prefix to identify character sprites (e.g., 'character').")]
        public string CharacterSpritePrefix = "character";

        [Tooltip("Prefix to identify card sprites (e.g., 'card').")]
        public string CardSpritePrefix = "card";

        [Tooltip("Prefix to identify status effect icons (e.g., 'status').")]
        public string StatusEffectIconPrefix = "status";

        [FolderPath] [Tooltip("Path to the folder where card sprites are stored.")]
        public string CardSpriteFolderPath = "Assets/Resources/Sprites/Cards";

        [FolderPath] [Tooltip("Path to the folder where character sprites are stored.")]
        public string CharacterSpriteFolderPath = "Assets/Resources/Sprites/Characters";

        [FolderPath]
        [Tooltip("Path to the folder where status effect icons are stored.")]
        public string StatusEffectIconFolderPath = "Assets/Resources/Sprites/StatusEffects";

        public static SpritePipelineSettings Instance
        {
            get
            {
                if (_instance) return _instance;
                _instance = Resources.Load<SpritePipelineSettings>("SpritePipelineSettings");
                if (_instance == null)
                    Debug.LogError("SpritePipelineSettings asset not found in Resources folder.");
                return _instance;
            }
        }
    }
}