using System;
using Runtime.CardGameplay.Card;
using Runtime.CardGameplay.Card.CardBehaviour;
using Runtime.Combat;
using Runtime.Combat.Pawn;
using Runtime.Combat.StatusEffects;
using Runtime.RunManagement;
using Runtime.UI.Tooltip;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace Editor.CardManager
{
    public sealed class GameplayDataEditor : OdinMenuEditorWindow
    {
        private static readonly Type[] TypesToDisplay = { typeof(CardData), typeof(PawnData), typeof(School) };
        private Type _selectedType;

        [MenuItem("Tools/Gameplay Data Editor")]
        private static void Open()
        {
            GetWindow<GameplayDataEditor>();
        }

        protected override void OnImGUI()
        {
            // Add the "Create New Asset" button above the tree, with decoration
            GUILayout.Space(10);
            GUILayout.Label("✦ Create New Asset ✦", new GUIStyle(EditorStyles.boldLabel)
            {
                fontSize = 16,
                alignment = TextAnchor.MiddleCenter,
                normal = { textColor = Color.green }
            });
            GUILayout.Space(5);

            if (GUILayout.Button("Create New Asset", GUILayout.Height(30))) ShowCreateNewAssetPopup();

            GUILayout.Space(10);
            base.OnImGUI();
        }

        protected override OdinMenuTree BuildMenuTree()
        {
            var tree = new OdinMenuTree { DefaultMenuStyle = { IconSize = 28.00f } };

            foreach (var type in TypesToDisplay)
            {
                var tabName = type.Name switch
                {
                    nameof(CardData) => "Cards",
                    nameof(PawnData) => "Pawns",
                    nameof(School) => "Schools",
                    _ => type.Name
                };

                tree.Add(tabName, new object()); // Header

                if (type == typeof(CardData))
                {
                    tree.Add("Cards/All", new object());
                    tree.AddAllAssetsAtPath("Cards/All", "Assets/Resources/Data", typeof(CardData), true, true);
                    AddCardsGroupedBySchool(tree, "Cards", "Assets/Resources/Data");
                }
                else
                {
                    tree.AddAllAssetsAtPath(tabName, "Assets/Resources/Data", type, true, true);
                }
            }

            return tree;
        }

        private static void AddCardsGroupedBySchool(OdinMenuTree tree, string root, string dataPath)
        {
            var schools = AssetDatabase.FindAssets("t:School", new[] { dataPath });

            foreach (var guid in schools)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var school = AssetDatabase.LoadAssetAtPath<School>(path);
                if (school == null) continue;

                foreach (var card in school.StarterCards) AddCard(card);
                foreach (var card in school.CollectableCards) AddCard(card);
                continue;

                void AddCard(CardData card)
                {
                    if (!card) return;
                    var menuPath = $"{root}/{school.name}/{card.name}";
                    tree.Add(menuPath, card);
                }
            }
        }


        private static void ShowCreateNewAssetPopup()
        {
            // Display a popup for user to select a type and specify a name
            var window = CreateInstance<CreateNewAssetPopup>();
            window.Init(TypesToDisplay, "Assets/Resources/Data");
            window.titleContent = new GUIContent("Create New Asset");
            window.ShowUtility();
        }
    }

    public class CreateNewAssetPopup : EditorWindow
    {
        private string _assetName = "NewAsset";
        private string _path;
        private Type _selectedType;

        private Type[] _typesToDisplay;

        private void OnGUI()
        {
            GUILayout.Label("Create New Asset", EditorStyles.boldLabel);
            GUILayout.Space(10);

            // Type Selection Dropdown
            GUILayout.Label("Select Asset Type", EditorStyles.label);
            foreach (var type in _typesToDisplay)
                if (GUILayout.Button(type.Name))
                    _selectedType = type;

            GUILayout.Space(10);

            // Asset Name Input
            GUILayout.Label("Specify Asset Name", EditorStyles.label);
            _assetName = EditorGUILayout.TextField("Asset Name", _assetName);

            GUILayout.Space(10);

            // Create Asset Button
            if (_selectedType != null && !string.IsNullOrWhiteSpace(_assetName))
            {
                if (GUILayout.Button("Create", GUILayout.Height(30))) CreateAsset();
            }
            else
            {
                EditorGUILayout.HelpBox("Select a type and provide a valid asset name.", MessageType.Warning);
            }
        }

        public void Init(Type[] typesToDisplay, string path)
        {
            _typesToDisplay = typesToDisplay;
            _path = path;
        }

        private void CreateAsset()
        {
            // Determine the proper subfolder based on the selected type
            var subfolder = GetSubfolderName(_selectedType);

            if (string.IsNullOrEmpty(subfolder))
            {
                Debug.LogError($"No folder mapping found for type {_selectedType.Name}. Cannot create asset.");
                return;
            }

            // Construct the full path, ensuring it's placed in the correct subfolder
            var uniquePath = AssetDatabase.GenerateUniqueAssetPath($"{_path}/{subfolder}/{_assetName}.asset");

            // Create the asset and save it to the specified location
            var asset = CreateInstance(_selectedType);

            switch (asset)
            {
                case CardData cardData:
                    cardData.Title = _assetName;
                    break;
                case PawnData pawnData:
                    pawnData.Title = _assetName;
                    break;
            }

            AssetDatabase.CreateAsset(asset, uniquePath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            // Highlight the new asset in the Project view
            Selection.activeObject = asset;

            // Refresh the GameplayDataEditor window if it's open
            var editorWindow = GetWindow<GameplayDataEditor>();
            if (editorWindow) editorWindow.ForceMenuTreeRebuild();

            Debug.Log($"Created new asset of type {_selectedType.Name} at: {uniquePath}");
            Close();
        }

        private static string GetSubfolderName(Type type)
        {
            return type.Name switch
            {
                nameof(CardData) => "Cards",
                nameof(PawnData) => "Pawns",
                nameof(School) => "Schools",
                nameof(CardPlayStrategy) => "Strategies",
                nameof(StatusEffectData) => "StatusEffects",
                nameof(CombatConfig) => "Combats",
                nameof(TooltipData) => "Tooltips",
                _ => null // Return null if no mapping is found
            };
        }
    }
}