using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Editor.ArtPipeline
{
    public class SpritePipelineValidation : AssetPostprocessor
    {
        private static SpritePipelineSettings _settings;

        private void OnPreprocessTexture()
        {
            if (_settings == null) _settings = LoadPipelineSettings();

            // Validate assets only in the "Import" folder
            if (!assetPath.StartsWith("Assets/Sprites/Import")) return;

            var texture = AssetDatabase.LoadAssetAtPath<Texture2D>(assetPath);
            if (texture == null)
            {
                Debug.LogError($"[Validation Error] Unable to load texture at path: {assetPath}");
                return;
            }

            Debug.Log($"[Validation Started] Processing asset: {assetPath}");

            // Perform the validation
            if (!ValidateSprite(texture, assetPath))
            {
                Debug.LogWarning(
                    $"[Validation Failed] Asset '{assetPath}' has validation issues. Please review the logs for details.");
                return; // Stop further processing, keep the file as is
            }

            Debug.Log($"[Validation Passed] Asset '{assetPath}' successfully passed all validation checks.");
        }

        private static SpritePipelineSettings LoadPipelineSettings()
        {
            // Load the settings from Resources folder
            var settings = Resources.Load<SpritePipelineSettings>("SpritePipelineSettings");
            if (settings == null)
                Debug.LogError(
                    "[Pipeline Error] SpritePipelineSettings file not found in 'Assets/Resources'. Please create one.");

            return settings;
        }

        private static bool ValidateSprite(Texture2D texture, string assetPath)
        {
            var isValid = true;
            var fileName = Path.GetFileNameWithoutExtension(assetPath);

            // 1. Validate dimensions
            if (texture.width < _settings.MinWidth || texture.width > _settings.MaxWidth ||
                texture.height < _settings.MinHeight || texture.height > _settings.MaxHeight)
            {
                Debug.LogError(
                    $"[Validation Failed: Dimensions] Asset '{fileName}' has invalid dimensions ({texture.width}x{texture.height}). " +
                    $"Expected dimensions between {_settings.MinWidth}x{_settings.MinHeight} and {_settings.MaxWidth}x{_settings.MaxHeight}.");
                isValid = false;
            }

            // 2. Validate naming convention
            if (_settings.EnforceNamingConvention && !ValidateNamingConvention(fileName)) isValid = false;

            // 3. Validate file format
            var extension = Path.GetExtension(assetPath).ToLower();
            if (!_settings.AllowedExtensions.Contains(extension))
            {
                Debug.LogError(
                    $"[Validation Failed: Format] Asset '{fileName}' has an invalid file format '{extension}'. " +
                    $"Allowed formats: {string.Join(", ", _settings.AllowedExtensions)}.");
                isValid = false;
            }

            return isValid; // Return the overall validation result
        }

        private static bool ValidateNamingConvention(string fileName)
        {
            var parts = fileName.Split('_');

            // Ensure the file name has 3 parts: type_name_version
            if (parts.Length != 3)
            {
                Debug.LogError(
                    $"[Validation Failed: Naming] Asset '{fileName}' must follow the naming convention 'type_name_version'.");
                return false;
            }

            var type = parts[0].ToLower(); // Type of the asset
            var version = parts[2].ToLower(); // Version of the asset

            // Validate the 'type'
            if (!_settings.AllowedTypes.Contains(type))
            {
                Debug.LogError(
                    $"[Validation Failed: Naming | Type] Asset '{fileName}' has an invalid type '{type}'. Allowed types: {string.Join(",", _settings.AllowedTypes)}.");
                return false;
            }

            // Validate the 'version'
            if (!_settings.AllowedVersions.Contains(version))
            {
                Debug.LogError(
                    $"[Validation Failed: Naming | Version] Asset '{fileName}' has an invalid version '{version}'. Allowed versions: {string.Join(", ", _settings.AllowedVersions)}.");
                return false;
            }

            return true; // Naming passed
        }
    }
}