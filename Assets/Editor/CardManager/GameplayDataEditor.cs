using System.Collections.Generic;
using System.IO;
using Runtime.CardGameplay.Card;
using Runtime.Combat.Pawn;
using Runtime.RunManagement;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;
// ← for Directory
// ← for AssetDatabase, Selection

namespace Editor.CardManager
{
    public sealed class GameplayDataEditor : OdinEditorWindow
    {
        [TabGroup("Cards")]
        [TableList(AlwaysExpanded = true, DrawScrollView = true, ShowPaging = true)]
        [SerializeField]
        private List<CardData> _cards;

        [TabGroup("Pawns")]
        [TableList(AlwaysExpanded = true, DrawScrollView = true, ShowPaging = true)]
        [SerializeField]
        private List<PawnData> _pawns;

        [TabGroup("Schools")]
        [TableList(AlwaysExpanded = true, DrawScrollView = true, ShowPaging = true, ShowIndexLabels = true)]
        [SerializeField]
        private List<School> _schools;

        private void OnEnable()
        {
            LoadAllData();
        }

        [MenuItem("Tools/Gameplay Data Editor")]
        private static void Open()
        {
            GetWindow<GameplayDataEditor>("Gameplay Data");
        }

        [Button(ButtonSizes.Large)]
        [PropertyOrder(-10)]
        private void Refresh()
        {
            LoadAllData();
        }

        private void LoadAllData()
        {
            _cards = FindAssets<CardData>();
            _pawns = FindAssets<PawnData>();
            _schools = FindAssets<School>();
        }

        private static List<T> FindAssets<T>() where T : ScriptableObject
        {
            var guids = AssetDatabase.FindAssets($"t:{typeof(T).Name}");
            var result = new List<T>(guids.Length);

            foreach (var g in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(g);
                var obj = AssetDatabase.LoadAssetAtPath<T>(path);
                if (obj != null) result.Add(obj);
            }

            return result;
        }

        // ───────────────────────────────────────────
        //   NEW ASSET CREATION BUTTONS
        // ───────────────────────────────────────────

        [TabGroup("Cards")]
        [Button("Create New Card", ButtonSizes.Medium)]
        private void CreateNewCard(string cardName)
        {
            const string folder = "Assets/Resources/Data/Cards";
            if (!AssetDatabase.IsValidFolder(folder)) Directory.CreateDirectory(folder);

            var asset = CreateInstance<CardData>();
            asset.Title = cardName;
            var path = $"{folder}/{cardName}.asset";

            AssetDatabase.CreateAsset(asset, path);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            LoadAllData();
            Selection.activeObject = asset;
        }

        [TabGroup("Pawns")]
        [Button("Create New Pawn", ButtonSizes.Medium)]
        private void CreateNewPawn(string pawnName, bool createSummonCard, int cost)
        {
            const string folder = "Assets/Resources/Data/Pawns";
            if (!AssetDatabase.IsValidFolder(folder)) Directory.CreateDirectory(folder);

            var asset = CreateInstance<PawnData>();
            asset.Title = pawnName;
            var path = $"{folder}/{pawnName}.asset";

            AssetDatabase.CreateAsset(asset, path);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            if (createSummonCard) asset.CreateSummonCard(cost);

            LoadAllData();
            Selection.activeObject = asset;
        }

        [TabGroup("Schools")]
        [Button("Create New School", ButtonSizes.Medium)]
        private void CreateNewSchool(string schoolName)
        {
            const string folder = "Assets/Resources/Data/Schools";
            if (!AssetDatabase.IsValidFolder(folder)) Directory.CreateDirectory(folder);

            var asset = CreateInstance<School>();
            var path = $"{folder}/{schoolName}.asset";

            AssetDatabase.CreateAsset(asset, path);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            LoadAllData();
            Selection.activeObject = asset;
        }
    }
}