using System.Collections.Generic;
using System.IO;
using System.Linq;
using Runtime.CardGameplay.Card;
using Runtime.Combat.Pawn;
using UnityEditor;
using UnityEngine;
// For PawnData

// For CardData

namespace Editor.ArtPipeline
{
    public class ArtStatusWindow : EditorWindow
    {
        private const string DataFolderPath = "Assets/Resources/Data";
        private List<ArtStatus> _artStatusList;

        private void OnGUI()
        {
            if (_artStatusList == null)
            {
                if (GUILayout.Button("Refresh Art Status")) RefreshArtStatus();

                return;
            }

            GUILayout.Label("Art Status for Project", EditorStyles.boldLabel);

            // Header Row
            GUILayout.BeginHorizontal();
            GUILayout.Label("Asset Type", GUILayout.Width(100));
            GUILayout.Label("Asset Name", GUILayout.Width(200));
            GUILayout.Label("Art Status", GUILayout.Width(250));
            GUILayout.EndHorizontal();

            EditorGUILayout.Space();

            // Display Art Status List
            foreach (var status in _artStatusList)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label(status.AssetType, GUILayout.Width(100));
                GUILayout.Label(status.AssetName, GUILayout.Width(200));
                GUILayout.Label(status.Status, GUILayout.Width(250));
                GUILayout.EndHorizontal();
            }

            EditorGUILayout.Space();

            // Export Button
            if (GUILayout.Button("Export as CSV")) ExportAsCsv();
        }

        [MenuItem("Tools/Art Pipeline/Art Status Listing")]
        public static void ShowWindow()
        {
            var window = GetWindow<ArtStatusWindow>("Art Status Listing");
            window.RefreshArtStatus();
        }

        private void RefreshArtStatus()
        {
            _artStatusList = new List<ArtStatus>();

            // Load CardData assets
            var cardDataAssets = LoadAllAssets<CardData>();
            foreach (var card in cardDataAssets)
            {
                var artStatus = GetArtStatus(card.Image);
                _artStatusList.Add(new ArtStatus
                {
                    AssetType = "CardData",
                    AssetName = card.name,
                    Status = artStatus
                });
            }

            // Load PawnData assets
            var pawnDataAssets = LoadAllAssets<PawnData>();
            foreach (var pawn in pawnDataAssets)
            {
                var artStatus = GetArtStatus(pawn.Sprite);
                _artStatusList.Add(new ArtStatus
                {
                    AssetType = "PawnData",
                    AssetName = pawn.name,
                    Status = artStatus
                });
            }

            Debug.Log("[ArtStatusWindow] Art status listing refreshed!");
        }

        private void ExportAsCsv()
        {
            var path = EditorUtility.SaveFilePanel("Export Art Status as CSV", "", "ArtStatus.csv", "csv");
            if (string.IsNullOrEmpty(path)) return;

            using (var writer = new StreamWriter(path))
            {
                writer.WriteLine("Asset Type,Asset Name,Art Status");

                foreach (var status in _artStatusList)
                    writer.WriteLine($"{status.AssetType},{status.AssetName},{status.Status}");
            }

            Debug.Log($"[ArtStatusWindow] Art status exported to: {path}");
        }

        private static string GetArtStatus(Sprite sprite)
        {
            if (sprite == null) return "Missing";

            var version = ExtractVersion(sprite.name);
            return !string.IsNullOrEmpty(version) ? $"Assigned (Version: {version})" : "Assigned (No Version)";
        }

        private static string ExtractVersion(string name)
        {
            var versionTag = name.ToLower()
                .Split('_')
                .FirstOrDefault(s => s.StartsWith("v") && int.TryParse(s.Substring(1), out _));

            return versionTag; // Returns null if no version
        }

        private static T[] LoadAllAssets<T>() where T : Object
        {
            var guids = AssetDatabase.FindAssets($"t:{typeof(T).Name}", new[] { DataFolderPath });
            return guids.Select(guid => AssetDatabase.LoadAssetAtPath<T>(AssetDatabase.GUIDToAssetPath(guid)))
                .ToArray();
        }

        private class ArtStatus
        {
            public string AssetType { get; set; }
            public string AssetName { get; set; }
            public string Status { get; set; }
        }
    }
}