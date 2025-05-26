using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Editor.ArtPipeline
{
    public static class SpritePipelineEditorTools
    {
        private const string ImportFolderPath = "Assets/Sprites/Import/";

        [MenuItem("Tools/Art Pipeline/Validate All Sprites in Import Folder")]
        public static void ValidateAllSprites()
        {
            if (!Directory.Exists(ImportFolderPath))
            {
                Debug.LogError($"[SpritePipelineEditorTools] Import folder does not exist at '{ImportFolderPath}'.");
                return;
            }

            // Find all textures in the Import folder
            var assetPaths = Directory.GetFiles(ImportFolderPath, "*.*", SearchOption.AllDirectories);
            foreach (var path in assetPaths)
                if (path.EndsWith(".png") || path.EndsWith(".jpg"))
                    ValidateSpriteAtPath(path);

            Debug.Log("[SpritePipelineEditorTools] Validation completed for all sprites in the Import folder.");
        }

        [MenuItem("Tools/Art Pipeline/Sort All Sprites in Import Folder")]
        public static void SortAllSprites()
        {
            if (!Directory.Exists(ImportFolderPath))
            {
                Debug.LogError($"[SpritePipelineEditorTools] Import folder does not exist at '{ImportFolderPath}'.");
                return;
            }

            // Find all textures in the Import folder
            var assetPaths = Directory.GetFiles(ImportFolderPath, "*.*", SearchOption.AllDirectories);
            foreach (var path in assetPaths)
                if (path.EndsWith(".png") || path.EndsWith(".jpg"))
                {
                    var relativePath = path.Substring(path.IndexOf("Assets/", StringComparison.Ordinal));
                    SpriteSorter.SortAndAssignSprites(relativePath);
                }

            Debug.Log("[SpritePipelineEditorTools] Sorting completed for all sprites in the Import folder.");
        }

        [MenuItem("Tools/Art Pipeline/Validate and Sort Selected Sprites")]
        public static void ValidateAndSortSelectedSprites()
        {
            foreach (var obj in Selection.objects)
            {
                var assetPath = AssetDatabase.GetAssetPath(obj);
                if (string.IsNullOrEmpty(assetPath)) continue;

                // Validation
                if (AssetDatabase.GetMainAssetTypeAtPath(assetPath) == typeof(Texture2D))
                {
                    ValidateSpriteAtPath(assetPath);
                    SpriteSorter.SortAndAssignSprites(assetPath);
                }
                else
                {
                    Debug.LogWarning($"[SpritePipelineEditorTools] Skipping non-texture asset: {assetPath}");
                }
            }

            Debug.Log("[SpritePipelineEditorTools] Validation and sorting completed for selected sprites.");
        }

        private static void ValidateSpriteAtPath(string assetPath)
        {
            var texture = AssetDatabase.LoadAssetAtPath<Texture2D>(assetPath);
            if (texture == null)
            {
                Debug.LogError(
                    $"[SpritePipelineEditorTools] Failed to load texture at {assetPath}. Skipping validation.");
                return;
            }

            // Perform validation using SpritePipelineValidation
            Debug.Log($"[SpritePipelineEditorTools] Validating sprite at {assetPath}...");
            var isValid = SpritePipelineValidation.ValidateSprite(texture, assetPath);
            if (isValid)
                Debug.Log($"[SpritePipelineEditorTools] Validation passed for sprite at {assetPath}.");
            else
                Debug.LogError($"[SpritePipelineEditorTools] Validation failed for sprite at {assetPath}.");
        }
    }
}