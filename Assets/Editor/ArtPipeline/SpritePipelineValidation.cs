using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

// Required for HashSet

namespace Editor.ArtPipeline
{
    public class SpritePipelineValidation : AssetPostprocessor
    {
        private static readonly SpritePipelineSettings Settings = SpritePipelineSettings.Instance;
        private static readonly HashSet<string> ProcessedAssets = new();

        private void OnPostprocessTexture(Texture2D texture)
        {
            var standardizedPath = assetPath.Replace("\\", "/");

            // Avoid re-processing assets that have already been handled
            if (ProcessedAssets.Contains(standardizedPath)) return;

            ProcessedAssets.Add(standardizedPath);

            // Validation and sorting logic here...
        }


        internal static bool ValidateSprite(Texture2D texture, string assetPath)
        {
            var isValid = true;
            var fileName = Path.GetFileNameWithoutExtension(assetPath);

            // 1. Validate dimensions
            if (texture.width < Settings.MinWidth || texture.width > Settings.MaxWidth ||
                texture.height < Settings.MinHeight || texture.height > Settings.MaxHeight)
            {
                Debug.LogError(
                    $"[Validation Failed: Dimensions] Asset '{fileName}' has invalid dimensions ({texture.width}x{texture.height}). " +
                    $"Expected dimensions between {Settings.MinWidth}x{Settings.MinHeight} and {Settings.MaxWidth}x{Settings.MaxHeight}.");
                isValid = false;
            }

            // 2. Validate naming convention
            if (!ValidateNamingConvention(fileName)) isValid = false;

            // 3. Validate file format
            var extension = Path.GetExtension(assetPath).ToLower();
            if (!Settings.AllowedExtensions.Contains(extension))
            {
                Debug.LogError(
                    $"[Validation Failed: Format] Asset '{fileName}' has an invalid file format '{extension}'. " +
                    $"Allowed formats: {string.Join(", ", Settings.AllowedExtensions)}.");
                isValid = false;
            }

            return isValid; // Return the overall validation result
        }

        private static bool ValidateNamingConvention(string fileName)
        {
            // File name validation format: {Type}_{Name}_v{Version}
            var parts = fileName.Split('_');
            if (parts.Length != 3)
            {
                Debug.LogError(
                    $"[Validation Failed: Naming] Asset '{fileName}' must follow the naming format: Type_Name_vVersion.");
                return false;
            }

            // Validate the type prefix
            var type = parts[0].ToLower();
            if (type != Settings.CardSpritePrefix.ToLower() && type != Settings.CardSpritePrefix.ToLower())
            {
                Debug.LogError(
                    $"[Validation Failed: Naming] Asset '{fileName}' has an invalid type prefix '{type}'. Expected: '{Settings.CardSpritePrefix}' or '{Settings.CardSpritePrefix}'.");
                return false;
            }

            // Validate the version suffix (vN)
            var versionSegment = parts[2].ToLower();
            if (!versionSegment.StartsWith("v") || !int.TryParse(versionSegment.Substring(1), out _))
            {
                Debug.LogError(
                    $"[Validation Failed: Naming] Asset '{fileName}' must contain a valid version suffix (e.g., v1, v2).");
                return false;
            }

            return true; // Naming passed
        }
    }
}