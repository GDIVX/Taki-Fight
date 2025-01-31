using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Runtime.CardGameplay.Card;

namespace Editor.ArtAssetsPipeline
{
    public class CardDataImportHandler : IArtImportHandler
    {
        private readonly ArtImporterSettings _settings;

        public CardDataImportHandler(ArtImporterSettings settings)
        {
            _settings = settings;
        }

        public bool CanHandle(string assetPath)
        {
            string fileName = Path.GetFileNameWithoutExtension(assetPath);
            Debug.Log($"CardDataImportHandler: Checking if can handle file: {fileName}");
            var fileNameSuffixOrPattern = _settings.ImportRules
                .Find(r => r.Type == ArtImportRule.ImportType.Artwork)?.FileNameSuffixOrPattern;
            return fileNameSuffixOrPattern != null &&
                   !string.IsNullOrEmpty(fileName) &&
                   fileName.ToLower().EndsWith(fileNameSuffixOrPattern);
        }

        public void Handle(string assetPath)
        {
            Debug.Log($"CardDataImportHandler: Handling file: {assetPath}");
            string fileName = Path.GetFileNameWithoutExtension(assetPath);
            if (string.IsNullOrEmpty(fileName))
            {
                Debug.LogError("CardDataImportHandler.Handle: fileName is null or empty.");
                return;
            }

            var parts = fileName.Split('_');
            if (parts.Length != 2)
            {
                Debug.LogError(
                    $"CardDataImportHandler: Invalid file name '{fileName}'. Expected format: 'Name_Artwork'.");
                return;
            }

            string cardName = parts[0];
            Debug.Log($"CardDataImportHandler: Extracted card name '{cardName}'");

            string[] guids = AssetDatabase.FindAssets($"t:{nameof(CardData)}");
            CardData cardData = null;

            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                Debug.Log($"CardDataImportHandler: Checking asset at {path}");
                CardData data = AssetDatabase.LoadAssetAtPath<CardData>(path);
                if (data != null && data.Title == cardName)
                {
                    cardData = data;
                    Debug.Log($"CardDataImportHandler: Found matching CardData for '{cardName}' at {path}");
                    break;
                }
            }

            if (cardData == null)
            {
                Debug.LogError($"CardDataImportHandler: No CardData found for card name '{cardName}'.");
                return;
            }

            string extension = Path.GetExtension(assetPath);
            var exportFolder = _settings.ImportRules
                .Find(r => r.Type == ArtImportRule.ImportType.Artwork)?.ExportFolder;
            if (exportFolder != null)
            {
                string outputPath = Path.Combine(exportFolder, $"{fileName}{extension}")
                    .Replace('\\', '/');
                string normalizedAssetPath = assetPath.Replace('\\', '/');

                Debug.Log($"CardDataImportHandler: Output path resolved to '{outputPath}'");

                if (normalizedAssetPath.Equals(outputPath, System.StringComparison.OrdinalIgnoreCase))
                {
                    Debug.Log($"CardDataImportHandler: Asset is already in the correct location: {outputPath}");
                }
                else
                {
                    if (File.Exists(outputPath))
                    {
                        Debug.LogWarning(
                            $"CardDataImportHandler: File already exists at '{outputPath}', deleting old file.");
                        AssetDatabase.DeleteAsset(outputPath);
                    }

                    string moveError = AssetDatabase.MoveAsset(assetPath, outputPath);
                    if (!string.IsNullOrEmpty(moveError))
                    {
                        Debug.LogError($"CardDataImportHandler: Failed to move asset: {moveError}");
                        return;
                    }
                }

                Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(outputPath);
                if (sprite == null)
                {
                    Debug.LogError($"CardDataImportHandler: Failed to load sprite at '{outputPath}'.");
                    return;
                }

                SerializedObject serializedCardData = new SerializedObject(cardData);
                SerializedProperty imageProperty = serializedCardData.FindProperty("_image");
                imageProperty.objectReferenceValue = sprite;
                serializedCardData.ApplyModifiedProperties();
            }

            Debug.Log($"CardDataImportHandler: Successfully injected artwork into CardData '{cardName}'");
        }
    }
}