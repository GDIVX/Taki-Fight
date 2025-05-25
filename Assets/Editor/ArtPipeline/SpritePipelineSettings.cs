using UnityEngine;

namespace Editor.ArtPipeline
{
    [CreateAssetMenu(fileName = "SpritePipelineSettings", menuName = "Pipeline/Sprite Pipeline Settings", order = 0)]
    public class SpritePipelineSettings : ScriptableObject
    {
        [Header("Size Constraints")] [Tooltip("Minimum allowed width for sprites.")]
        public int MinWidth = 128;

        [Tooltip("Maximum allowed width for sprites.")]
        public int MaxWidth = 2048;

        [Tooltip("Minimum allowed height for sprites.")]
        public int MinHeight = 128;

        [Tooltip("Maximum allowed height for sprites.")]
        public int MaxHeight = 2048;

        [Header("File Validation")] [Tooltip("Allowed file extensions, e.g., .png, .jpg")]
        public string[] AllowedExtensions = { ".png", ".jpg" };

        [Header("Naming Conventions")] [Tooltip("Check if names must follow specific rules, e.g., no spaces.")]
        public bool EnforceNamingConvention = true;

        [Tooltip("Include assets with these keywords in the name.")]
        public string[] RequiredNameKeywords = { };

        [Tooltip("Exclude assets with these forbidden keywords in the name.")]
        public string[] ForbiddenNameKeywords = { };

        [Header("Naming Convention Settings")]
        [Tooltip("Allowed types for the naming convention (e.g., 'character', 'card').")]
        public string[] AllowedTypes = { "character", "card" };

        [Tooltip("Allowed versions for the naming convention (e.g., 'temp', 'final').")]
        public string[] AllowedVersions = { "temp", "final" };
    }
}