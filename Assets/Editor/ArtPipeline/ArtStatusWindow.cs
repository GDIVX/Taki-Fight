using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Runtime.CardGameplay.Card;
using Runtime.Combat.Pawn;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Editor.ArtPipeline
{
    public class ArtStatusWindow : OdinEditorWindow
    {
        private const string DataFolderPath = "Assets/Resources/Data";
        private const string ImportFolderPath = "Assets/Sprites/Import";

        [TableList(AlwaysExpanded = true, ShowPaging = false, DrawScrollView = true)] [ShowInInspector]
        private List<ArtStatus> _artStatusList = new();

        [MenuItem("Tools/Art Pipeline/Art Status Listing")]
        public static void ShowWindow()
        {
            var window = GetWindow<ArtStatusWindow>("Art Status Listing");
            window.RefreshArtStatus();
        }

        [Button(ButtonSizes.Large)]
        [GUIColor(0.4f, 0.8f, 1.0f)]
        public void RefreshArtStatus()
        {
            _artStatusList.Clear();

            // Load CardData assets
            var cardDataAssets = LoadAllAssets<CardData>();
            foreach (var card in cardDataAssets)
            {
                _artStatusList.Add(new ArtStatus
                {
                    AssetType = "CardData",
                    AssetName = card.name,
                    SpriteReference = card.Image,
                    DataObject = card
                });
            }

            // Load PawnData assets
            var pawnDataAssets = LoadAllAssets<PawnData>();
            foreach (var pawn in pawnDataAssets)
            {
                _artStatusList.Add(new ArtStatus
                {
                    AssetType = "PawnData",
                    AssetName = pawn.name,
                    SpriteReference = pawn.Sprite,
                    DataObject = pawn
                });
            }

            Debug.Log("[ArtStatusWindow] Art status listing refreshed!");
        }

        private static T[] LoadAllAssets<T>() where T : Object
        {
            var guids = AssetDatabase.FindAssets($"t:{typeof(T).Name}", new[] { DataFolderPath });
            return guids.Select(guid => AssetDatabase.LoadAssetAtPath<T>(AssetDatabase.GUIDToAssetPath(guid)))
                .ToArray();
        }

        private static string GenerateAssetFileName(string assetType, string assetName, string fileExtension)
        {
            var prefix = assetType switch
            {
                "PawnData" => SpritePipelineSettings.Instance.CharacterSpritePrefix,
                "CardData" => SpritePipelineSettings.Instance.CardSpritePrefix,
                _ => "unknown"
            };
            var sanitizedAssetName = SanitizeFileName(assetName);
            var version = CalculateNextVersion(prefix, sanitizedAssetName);
            return $"{prefix}_{sanitizedAssetName}_{version}{fileExtension}";
        }

        private static string SanitizeFileName(string fileName)
        {
            return string.Join("_", fileName.Split(Path.GetInvalidFileNameChars())).Replace(" ", "_").ToLower();
        }

        private static string CalculateNextVersion(string prefix, string assetName)
        {
            var assetsInFolder = AssetDatabase.FindAssets("t:Sprite", new[] { ImportFolderPath })
                .Select(AssetDatabase.GUIDToAssetPath)
                .Where(p => Path.GetFileNameWithoutExtension(p).StartsWith($"{prefix}_{assetName}_"))
                .ToArray();
            var highestVersion = 0;

            foreach (var assetPath in assetsInFolder)
            {
                var versionSegment = ExtractVersion(Path.GetFileNameWithoutExtension(assetPath));
                if (versionSegment > highestVersion)
                    highestVersion = versionSegment;
            }

            return $"v{highestVersion + 1}";
        }

        private static int ExtractVersion(string name)
        {
            return int.TryParse(name.Split('_').LastOrDefault()?.Replace("v", ""), out var version) ? version : 0;
        }

        [TableList]
        [Serializable]
        public class ArtStatus
        {
            [TableColumnWidth(150, Resizable = false)] [PreviewField(Height = 50)]
            public Sprite SpriteReference;

            [ReadOnly] public string AssetType;

            [ReadOnly] public string AssetName;

            [HideInInspector] public Object DataObject; // CardData or PawnData reference

            [Button("Import Asset", ButtonSizes.Medium)]
            public void ImportAsset()
            {
                // Use a file dialog to allow the user to select a png/jpg
                var filePath = EditorUtility.OpenFilePanel($"Import Sprite for {AssetName}", "", "png,jpg");
                if (string.IsNullOrEmpty(filePath)) return;

                var fileExtension = Path.GetExtension(filePath).ToLower();
                if (fileExtension != ".png" && fileExtension != ".jpg")
                {
                    Debug.LogError(
                        $"[ArtStatusWindow] Invalid file format for {AssetName}. Only .png and .jpg are supported.");
                    return;
                }

                // Generate a proper name for the sprite using the asset type and name
                var newFileName = GenerateAssetFileName(AssetType, AssetName, fileExtension);
                var destinationPath = Path.Combine(ImportFolderPath, newFileName);

                // Ensure the import folder exists
                if (!Directory.Exists(ImportFolderPath))
                {
                    Directory.CreateDirectory(ImportFolderPath);
                    Debug.Log($"[ArtStatusWindow] Created Import folder at: {ImportFolderPath}");
                }

                // Copy and rename the file
                File.Copy(filePath, destinationPath, true);
                AssetDatabase.Refresh();

                // Automatically assign the imported sprite to the related data object
                var sprite = AssetDatabase.LoadAssetAtPath<Sprite>(destinationPath);
                if (sprite == null)
                {
                    Debug.LogError($"[ArtStatusWindow] Failed to load imported sprite at {destinationPath}.");
                    return;
                }

                if (DataObject is CardData cardData)
                    cardData.Image = sprite;
                else if (DataObject is PawnData pawnData) pawnData.Sprite = sprite;

                EditorUtility.SetDirty(DataObject);
                AssetDatabase.SaveAssets();

                Debug.Log(
                    $"[ArtStatusWindow] Successfully imported and assigned sprite '{sprite.name}' to {AssetName}.");
            }
        }
    }
}