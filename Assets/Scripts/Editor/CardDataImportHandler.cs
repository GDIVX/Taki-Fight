using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Runtime.CardGameplay.Card;

namespace Editor.ArtAssetsPipeline
{
    /// <summary>
    /// An implementation of IArtImportHandler that handles card artwork.
    /// Follows the naming convention "CardName_Artwork".
    /// </summary>
    public class CardDataImportHandler : IArtImportHandler
    {
        private readonly ArtImporterSettings _settings;

        public CardDataImportHandler(ArtImporterSettings settings)
        {
            _settings = settings;
        }

        public bool CanHandle(string assetPath)
        {
            // Ensure the path belongs to a file with the "_Artwork" suffix
            // Example file name: "Warrior_Artwork.png"
            string fileName = Path.GetFileNameWithoutExtension(assetPath);
            return !string.IsNullOrEmpty(fileName) &&
                   // We'll do a simple check: does it end with "_Artwork" (case-insensitive)?
                   fileName.ToLower().EndsWith(_settings.ImportRules
                       .Find(r => r.Type == ArtImportRule.ImportType.Artwork).FileNameSuffixOrPattern);
        }

        public void Handle(string assetPath)
        {
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

            string[] guids = AssetDatabase.FindAssets($"t:{nameof(CardData)}");
            CardData cardData = null;

            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                CardData data = AssetDatabase.LoadAssetAtPath<CardData>(path);
                if (data != null && data.Title == cardName)
                {
                    cardData = data;
                    break;
                }
            }

            if (cardData == null)
            {
                Debug.LogError($"CardDataImportHandler: No CardData found for card name '{cardName}'.");
                return;
            }

            string extension = Path.GetExtension(assetPath);
            string outputPath = Path.Combine(_settings.ImportRules
                        .Find(r => r.Type == ArtImportRule.ImportType.Artwork).ExportFolder, $"{fileName}{extension}")
                .Replace('\\', '/');
            string normalizedAssetPath = assetPath.Replace('\\', '/');

            if (normalizedAssetPath.Equals(outputPath, System.StringComparison.OrdinalIgnoreCase))
            {
                Debug.Log($"CardDataImportHandler: Asset is already in the correct location: {outputPath}");
            }
            else
            {
                // Check if the file already exists at the destination
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

            Debug.Log($"CardDataImportHandler: Successfully injected artwork into CardData '{cardName}'.");
        }
    }
}