using System.Collections.Generic;
using UnityEngine;

namespace Editor.ArtPipeline
{
    [CreateAssetMenu(fileName = "SpritePipelineSettings", menuName = "Pipeline/Sprite Pipeline Settings", order = 0)]
    public class SpritePipelineSettings : ScriptableObject
    {
        // Constants for default values
        private const string DefaultCharacterSpritePrefix = "character";
        private const string DefaultCardSpritePrefix = "card";
        private const int DefaultMinWidth = 128;
        private const int DefaultMaxWidth = 2048;
        private const int DefaultMinHeight = 128;
        private const int DefaultMaxHeight = 2048;
        private static SpritePipelineSettings _instance;

        [Header("Prefix Settings for Sprite Types")]
        [Tooltip("Prefix to identify character sprites (e.g., 'character').")]
        public string CharacterSpritePrefix = DefaultCharacterSpritePrefix;

        [Tooltip("Prefix to identify card sprites (e.g., 'card').")]
        public string CardSpritePrefix = DefaultCardSpritePrefix;

        [Header("Size Constraints")] [Tooltip("Minimum allowed width for sprites.")]
        public int MinWidth = DefaultMinWidth;

        [Tooltip("Maximum allowed width for sprites.")]
        public int MaxWidth = DefaultMaxWidth;

        [Tooltip("Minimum allowed height for sprites.")]
        public int MinHeight = DefaultMinHeight;

        [Tooltip("Maximum allowed height for sprites.")]
        public int MaxHeight = DefaultMaxHeight;

        [Header("File Extensions")] [Tooltip("List of allowed file extensions for sprites.")]
        public List<string> AllowedExtensions = new() { "png", "jpg" };

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