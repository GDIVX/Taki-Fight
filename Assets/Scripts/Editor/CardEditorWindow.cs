using System.Collections.Generic;
using System.Linq;
using Runtime.CardGameplay.Card;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    public class CardEditorWindow : EditorWindow
    {
        private enum EditorTab
        {
            Main,
            Advanced
        }

        private EditorTab _currentTab;

        private List<CardData> _allCards;
        private CardData _selectedCard;
        private Vector2 _cardListScrollPos;
        private string _searchFilter;

        [MenuItem("Window/Card Editor")]
        private static void OpenWindow()
        {
            var window = GetWindow<CardEditorWindow>("Card Editor");
            window.Show();
        }

        private void OnEnable()
        {
            LoadCards();
        }

        private void OnGUI()
        {
            DrawToolbar();
            GUILayout.Space(5);

            switch (_currentTab)
            {
                case EditorTab.Main:
                    DrawMainTab();
                    break;
                case EditorTab.Advanced:
                    DrawAdvancedTab();
                    break;
            }
        }

        private void DrawToolbar()
        {
            using (new EditorGUILayout.HorizontalScope(EditorStyles.toolbar))
            {
                if (GUILayout.Toggle(_currentTab == EditorTab.Main, "Main", EditorStyles.toolbarButton))
                    _currentTab = EditorTab.Main;
                if (GUILayout.Toggle(_currentTab == EditorTab.Advanced, "Advanced", EditorStyles.toolbarButton))
                    _currentTab = EditorTab.Advanced;
            }
        }

        private void DrawMainTab()
        {
            // Search bar
            _searchFilter = EditorGUILayout.TextField("Search", _searchFilter);

            using (new EditorGUILayout.HorizontalScope())
            {
                // Card list on the left
                using (var scroll = new EditorGUILayout.ScrollViewScope(_cardListScrollPos, GUILayout.Width(200)))
                {
                    _cardListScrollPos = scroll.scrollPosition;
                    var filtered = string.IsNullOrEmpty(_searchFilter)
                        ? _allCards
                        : _allCards.Where(c => c.Title.ToLower().Contains(_searchFilter.ToLower())).ToList();

                    foreach (var card in filtered)
                    {
                        if (GUILayout.Button(card.Title))
                            _selectedCard = card;
                    }

                    if (GUILayout.Button("Create New Card"))
                        CreateNewCard();
                }

                // Inspector + Preview on the right
                using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
                {
                    if (_selectedCard)
                    {
                        // Draw default inspector fields for CardData
                        var so = new SerializedObject(_selectedCard);
                        so.Update();
                        EditorGUILayout.PropertyField(so.FindProperty("_title"));
                        EditorGUILayout.PropertyField(so.FindProperty("_description"));
                        EditorGUILayout.PropertyField(so.FindProperty("_image"));
                        EditorGUILayout.PropertyField(so.FindProperty("_cardType"));
                        EditorGUILayout.PropertyField(so.FindProperty("_selectStrategy"));
                        EditorGUILayout.PropertyField(so.FindProperty("_playStrategies"), true);
                        EditorGUILayout.PropertyField(so.FindProperty("_destroyCardAfterUse"));
                        EditorGUILayout.PropertyField(so.FindProperty("_cost"));
                        EditorGUILayout.PropertyField(so.FindProperty("_extractGems"));
                        so.ApplyModifiedProperties();

                        GUILayout.Space(10);
                        EditorGUILayout.LabelField("Preview", EditorStyles.boldLabel);
                        // Basic preview (image + parsed text snippet)
                        if (_selectedCard.Image)
                            GUILayout.Label(_selectedCard.Image.texture, GUILayout.Width(128), GUILayout.Height(128));

                        // If you have a runtime card preview system, call or simulate it here:
                        EditorGUILayout.HelpBox($"Parsed text preview:\n{_selectedCard.Description}", MessageType.None);
                    }
                    else
                    {
                        EditorGUILayout.LabelField("Select a card to preview or create a new one.");
                    }
                }
            }
        }

        private void DrawAdvancedTab()
        {
            EditorGUILayout.LabelField("Batch Editing & Validation", EditorStyles.boldLabel);
            // Placeholder for advanced operations: 
            // e.g., bulk updating card types, validating placeholder usage, etc.
            if (GUILayout.Button("Validate All Cards"))
                ValidateCards();

            if (GUILayout.Button("Bulk Update"))
                BulkUpdateCards();
        }

        private void LoadCards()
        {
            // Loads all CardData in the project (you can refine this with custom logic)
            var guids = AssetDatabase.FindAssets("t:CardData");
            _allCards = new List<CardData>();
            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var card = AssetDatabase.LoadAssetAtPath<CardData>(path);
                if (card) _allCards.Add(card);
            }
        }

        private void CreateNewCard()
        {
            var newCard = ScriptableObject.CreateInstance<CardData>();
            newCard.name = "New Card";

            // Default path for CardData assets
            string defaultPath = "Assets/Resources/Data/Cards";
            if (!AssetDatabase.IsValidFolder(defaultPath))
            {
                // Ensure the directory exists
                string[] folders = defaultPath.Split('/');
                string currentPath = "Assets";
                foreach (string folder in folders.Skip(1))
                {
                    if (!AssetDatabase.IsValidFolder($"{currentPath}/{folder}"))
                        AssetDatabase.CreateFolder(currentPath, folder);
                    currentPath = $"{currentPath}/{folder}";
                }
            }

            // Save the new card in the default path
            string path = EditorUtility.SaveFilePanelInProject(
                "Save New Card",
                newCard.name,
                "asset",
                "Choose a location to save the new CardData asset",
                defaultPath);

            if (!string.IsNullOrEmpty(path))
            {
                AssetDatabase.CreateAsset(newCard, path);
                AssetDatabase.SaveAssets();
                LoadCards();
                _selectedCard = newCard;
            }
        }


        private void ValidateCards()
        {
            // Example: Check for placeholder or sprite issues
            // Show results in console or a small panel
            Debug.Log("Validation complete!");
        }

        private void BulkUpdateCards()
        {
            // Example: Apply a property change to all selected or filtered cards
            Debug.Log("Bulk update done!");
        }
    }
}