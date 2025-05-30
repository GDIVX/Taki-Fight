using System;
using System.IO;
using System.Linq;
using Runtime.CardGameplay.Card;
using Runtime.Combat.Pawn;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Editor.ArtPipeline
{
    public partial class ArtManagerWindow
    {
        [TableList]
        [Serializable]
        public class ArtEntry
        {
            [TableColumnWidth(150, Resizable = false)]
            [PreviewField(Height = 60, Alignment = ObjectFieldAlignment.Center)]
            public Sprite SpriteReference;

            [ReadOnly] public string Title;

            [ReadOnly] [TableColumnWidth(200)] public string SpriteName;

            [ReadOnly] public Object DataObject;

            internal string AssetName;

            internal string AssetType;

            [Button("Import Sprite", ButtonSizes.Medium)]
            [Tooltip("Import sprite from file and assign it to the selected asset.")]
            public void ImportAsset()
            {
                // Use a file dialog to allow the user to select a sprite (png or jpg)
                var importFilePath = EditorUtility.OpenFilePanel($"Import Sprite for {AssetName}", "", "png,jpg");
                if (string.IsNullOrEmpty(importFilePath)) return;

                var fileExtension = Path.GetExtension(importFilePath)?.ToLower();
                if (fileExtension != ".png" && fileExtension != ".jpg")
                {
                    Debug.LogError(
                        $"[ArtStatusWindow] Invalid file format for {AssetName}. Only .png and .jpg are supported.");
                    return;
                }

                // Generate a proper filename and move the sprite file
                var folderPath = GenerateDestinationPath(fileExtension, out var destinationPath);

                // Ensure the import folder exists
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                    Debug.Log($"[ArtStatusWindow] Created Import folder at: {folderPath}");
                }

                File.Copy(importFilePath, destinationPath, true);
                AssetDatabase.Refresh();

                // Automatically assign the imported sprite to the data object
                var sprite = AssetDatabase.LoadAssetAtPath<Sprite>(destinationPath);
                if (sprite == null)
                {
                    Debug.LogError($"[ArtStatusWindow] Failed to load imported sprite at {destinationPath}.");
                    return;
                }

                switch (DataObject)
                {
                    case CardData cardData:
                        cardData.Image = sprite;
                        break;
                    case PawnData pawnData:
                        pawnData.Sprite = sprite;
                        break;
                }

                SpriteReference = sprite;
                SpriteName = sprite.name;

                EditorUtility.SetDirty(DataObject);
                AssetDatabase.SaveAssets();

                Debug.Log(
                    $"[ArtStatusWindow] Successfully imported and assigned sprite '{sprite.name}' to {AssetName}.");
            }

            private string GenerateDestinationPath(string fileExtension, out string destinationPath)
            {
                var newFileName = GenerateAssetFileName(AssetType, AssetName, fileExtension);
                var folderPath = GetFolderPath(AssetType);
                destinationPath = Path.Combine(folderPath, newFileName);
                return folderPath;
            }


            [Button("Delete Outdated Sprites")]
            private void DeleteOldSpriteAssets()
            {
                if (SpriteReference == null || string.IsNullOrEmpty(SpriteName) || DataObject == null)
                {
                    Debug.LogWarning(
                        "[ArtStatusWindow] Skipping deletion since the current sprite or data object is invalid.");
                    return;
                }

                // Get the folder path
                var folderPath = GetFolderPath(AssetType);

                if (string.IsNullOrEmpty(folderPath) || !Directory.Exists(folderPath))
                {
                    Debug.LogWarning(
                        $"[ArtStatusWindow] Could not find folder path for asset type {AssetType}: {folderPath}");
                    return;
                }

                // Parse the current sprite name to extract base name and version
                if (!TryParseSpriteName(SpriteName, out var baseName, out var currentVersion))
                {
                    Debug.LogWarning($"[ArtStatusWindow] Could not parse sprite name for versioning: {SpriteName}");
                    return;
                }

                // List all sprite files in the folder
                var spriteFiles = Directory.GetFiles(folderPath, "*.png")
                    .Concat(Directory.GetFiles(folderPath, "*.jpg"))
                    .ToList();

                // Gather files eligible for deletion (older versions with the same base name)
                var filesToDelete = spriteFiles.Where(file =>
                {
                    var fileName = Path.GetFileNameWithoutExtension(file);
                    if (TryParseSpriteName(fileName, out var fileBaseName, out var fileVersion))
                        return fileBaseName == baseName && fileVersion < currentVersion;

                    return false;
                }).ToList();

                if (!filesToDelete.Any())
                {
                    Debug.Log($"[ArtStatusWindow] No older sprite versions found for: {SpriteName}");
                    return;
                }

                // Popup confirmation dialog
                if (!EditorUtility.DisplayDialog(
                        "Confirm Deletion",
                        $"Are you sure you want to delete {filesToDelete.Count} old sprite version(s) for: {SpriteName}?",
                        "Yes",
                        "No"))
                {
                    Debug.Log("[ArtStatusWindow] Deletion cancelled by the user.");
                    return;
                }

                // Proceed with deletion
                var failedFiles = new string[filesToDelete.Count];
                AssetDatabase.DeleteAssets(filesToDelete.ToArray(), filesToDelete);

                if (failedFiles.Any())
                {
                    Debug.LogError(
                        $"[ArtStatusWindow] Failed to delete the following files: {string.Join(", ", failedFiles)}");
                    return;
                }

                // Refresh the Asset Database
                AssetDatabase.Refresh();
                Debug.Log($"[ArtStatusWindow] Completed deletion of older versions for sprite: {SpriteName}");
            }

            private string GetFolderPath(string assetType)
            {
                return assetType switch
                {
                    "PawnData" => SpritePipelineSettings.Instance.CharacterSpriteFolderPath,
                    "CardData" => SpritePipelineSettings.Instance.CardSpriteFolderPath,
                    _ => "unknown"
                };
            }

            private bool TryParseSpriteName(string spriteName, out string baseName, out int version)
            {
                baseName = string.Empty;
                version = 0;

                // Look for a '_vX' at the end of the sprite name where X is the version number
                var versionSuffixIndex = spriteName.LastIndexOf("_v", StringComparison.OrdinalIgnoreCase);
                if (versionSuffixIndex < 0 ||
                    versionSuffixIndex >= spriteName.Length - 2) return false; // No valid version suffix found

                // Extract the base name and the version part
                baseName = spriteName.Substring(0, versionSuffixIndex);
                var versionPart = spriteName[(versionSuffixIndex + 2)..];

                // Try to parse the version part as an integer
                return int.TryParse(versionPart, out version);
            }
        }
    }
}