using System.Collections.Generic;
using System.IO;
using System.Linq;
using Runtime.CardGameplay.Card;
using Runtime.Combat.Pawn;
using Runtime.Combat.StatusEffects;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Editor.ArtPipeline
{
    public partial class ArtManagerWindow : OdinEditorWindow
    {
        private const string DataFolderPath = "Assets/Resources/Data";
        private const string SpritesFolderPath = "Assets/Sprites"; // Updated search folder scope

        [TableList(AlwaysExpanded = true, ShowPaging = false, DrawScrollView = true)] [Searchable] [ShowInInspector]
        private List<ArtEntry> _artStatusList = new();

        [MenuItem("Tools/Art Manager")]
        public static void ShowWindow()
        {
            var window = GetWindow<ArtManagerWindow>("Art Manager");
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
                _artStatusList.Add(new ArtEntry
                {
                    AssetType = "CardData",
                    AssetName = card.name,
                    Title = card.Title,
                    SpriteReference = card.Image,
                    SpriteName = FindSpriteAssetName(card.Image),
                    DataObject = card
                });
            }

            // Load PawnData assets
            var pawnDataAssets = LoadAllAssets<PawnData>();
            foreach (var pawn in pawnDataAssets)
            {
                _artStatusList.Add(new ArtEntry
                {
                    AssetType = "PawnData",
                    AssetName = pawn.name,
                    Title = pawn.Title,
                    SpriteReference = pawn.Sprite,
                    SpriteName = FindSpriteAssetName(pawn.Sprite),
                    DataObject = pawn
                });
            }

            // Load StatusEffectData assets
            var statusEffectDataAssets = LoadAllAssets<StatusEffectData>();
            foreach (var status in statusEffectDataAssets)
            {
                _artStatusList.Add(new ArtEntry
                {
                    AssetType = "StatusEffectData",
                    AssetName = status.name,
                    Title = status.Keyword ? status.Keyword.Header : status.name,
                    SpriteReference = status.Icon,
                    SpriteName = FindSpriteAssetName(status.Icon),
                    DataObject = status
                });
            }

            Debug.Log("[ArtStatusWindow] Art status listing refreshed!");
        }

        /// <summary>
        ///     Finds the file name of the given sprite by searching in the Sprites folder.
        ///     Returns the name of the sprite if found, or "None" if the sprite is not assigned.
        /// </summary>
        private static string FindSpriteAssetName(Sprite sprite)
        {
            if (!sprite)
                return "None";

            // Find the asset path of the sprite by name
            var assetPath = AssetDatabase.GetAssetPath(sprite);
            return string.IsNullOrEmpty(assetPath)
                ? "None"
                : Path.GetFileNameWithoutExtension(assetPath); // Return sprite name without extension
        }

        private static T[] LoadAllAssets<T>() where T : Object
        {
            var guids = AssetDatabase.FindAssets($"t:{typeof(T).Name}", new[] { DataFolderPath });
            return guids.Select(guid => AssetDatabase.LoadAssetAtPath<T>(AssetDatabase.GUIDToAssetPath(guid)))
                .ToArray();
        }

        private static string GenerateAssetFileName(string assetType, string assetName, string fileExtension,
            int version)
        {
            var prefix = assetType switch
            {
                "PawnData" => SpritePipelineSettings.Instance.CharacterSpritePrefix,
                "CardData" => SpritePipelineSettings.Instance.CardSpritePrefix,
                "StatusEffectData" => SpritePipelineSettings.Instance.StatusEffectIconPrefix,
                _ => "unknown"
            };
            var sanitizedAssetName = SanitizeFileName(assetName);
            return $"{prefix}_{sanitizedAssetName}_{version}{fileExtension}";
        }

        private static string GenerateAssetFileName(string assetType, string assetName, string fileExtension)
        {
            var prefix = assetType switch
            {
                "PawnData" => SpritePipelineSettings.Instance.CharacterSpritePrefix,
                "CardData" => SpritePipelineSettings.Instance.CardSpritePrefix,
                "StatusEffectData" => SpritePipelineSettings.Instance.StatusEffectIconPrefix,
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
    }
}