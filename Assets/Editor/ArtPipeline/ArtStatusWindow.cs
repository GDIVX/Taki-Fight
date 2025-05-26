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
        private const string SpritesFolderPath = "Assets/Sprites"; // Updated search folder scope

        [TableList(AlwaysExpanded = true, ShowPaging = false, DrawScrollView = true)] [ShowInInspector]
        private List<ArtStatus> ArtStatusList = new();

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
            ArtStatusList.Clear();

            // Load CardData assets
            var cardDataAssets = LoadAllAssets<CardData>();
            foreach (var card in cardDataAssets)
            {
                ArtStatusList.Add(new ArtStatus
                {
                    AssetType = "CardData",
                    AssetName = card.name,
                    SpriteReference = card.Image,
                    SpriteName = FindSpriteAssetName(card.Image),
                    DataObject = card
                });
            }

            // Load PawnData assets
            var pawnDataAssets = LoadAllAssets<PawnData>();
            foreach (var pawn in pawnDataAssets)
            {
                ArtStatusList.Add(new ArtStatus
                {
                    AssetType = "PawnData",
                    AssetName = pawn.name,
                    SpriteReference = pawn.Sprite,
                    SpriteName = FindSpriteAssetName(pawn.Sprite),
                    DataObject = pawn
                });
            }

            Debug.Log("[ArtStatusWindow] Art status listing refreshed!");
        }

        /// <summary>
        ///     Finds the file name of the given sprite by searching in the Sprites folder.
        ///     Returns the name of the sprite if found, or "None" if the sprite is not assigned.
        /// </summary>
        private string FindSpriteAssetName(Sprite sprite)
        {
            if (sprite == null)
                return "None";

            // Find the asset path of the sprite by name
            var assetPath = AssetDatabase.GetAssetPath(sprite);
            if (string.IsNullOrEmpty(assetPath))
                return "None";

            return Path.GetFileNameWithoutExtension(assetPath); // Return sprite name without extension
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
            // Updated search scope to Sprites folder
            var assetsInFolder = AssetDatabase.FindAssets("t:Sprite", new[] { SpritesFolderPath })
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

            [ReadOnly] [TableColumnWidth(200)] public string SpriteName; // Displays the sprite's file name

            [HideInInspector] public Object DataObject; // CardData or PawnData reference

            [Button("Import Asset", ButtonSizes.Medium)]
            public void ImportAsset()
            {
                // Use a file dialog to allow the user to select a sprite (png or jpg)
                var filePath = EditorUtility.OpenFilePanel($"Import Sprite for {AssetName}", "", "png,jpg");
                if (string.IsNullOrEmpty(filePath)) return;

                var fileExtension = Path.GetExtension(filePath)?.ToLower();
                if (fileExtension != ".png" && fileExtension != ".jpg")
                {
                    Debug.LogError(
                        $"[ArtStatusWindow] Invalid file format for {AssetName}. Only .png and .jpg are supported.");
                    return;
                }

                // Generate a proper filename and move the sprite file
                var newFileName = GenerateAssetFileName(AssetType, AssetName, fileExtension);
                var destinationPath = Path.Combine(SpritesFolderPath + "/Import", newFileName);

                // Ensure the import folder exists
                if (!Directory.Exists(SpritesFolderPath + "/Import"))
                {
                    Directory.CreateDirectory(SpritesFolderPath + "/Import");
                    Debug.Log($"[ArtStatusWindow] Created Import folder at: {SpritesFolderPath}/Import");
                }

                File.Copy(filePath, destinationPath, true);
                AssetDatabase.Refresh();

                // Automatically assign the imported sprite to the data object
                var sprite = AssetDatabase.LoadAssetAtPath<Sprite>(destinationPath);
                if (sprite == null)
                {
                    Debug.LogError($"[ArtStatusWindow] Failed to load imported sprite at {destinationPath}.");
                    return;
                }

                if (DataObject is CardData cardData)
                    cardData.Image = sprite;
                else if (DataObject is PawnData pawnData) pawnData.Sprite = sprite;

                SpriteReference = sprite;
                SpriteName = sprite.name;

                EditorUtility.SetDirty(DataObject);
                AssetDatabase.SaveAssets();

                Debug.Log(
                    $"[ArtStatusWindow] Successfully imported and assigned sprite '{sprite.name}' to {AssetName}.");
            }
        }
    }
}