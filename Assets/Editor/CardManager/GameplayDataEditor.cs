using System.Collections.Generic;
using System.Linq;
using Runtime.CardGameplay.Card;
using Runtime.Combat.Pawn;
using Runtime.RunManagement;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace Editor.CardManager
{
    public sealed class GameplayDataEditor : OdinEditorWindow
    {
        // ──────────────────────────────────  DATA  ──────────────────────────────────
        [TabGroup("Cards")] [TableList(AlwaysExpanded = true, DrawScrollView = true)] [SerializeField]
        private List<CardData> _cards;

        [TabGroup("Pawns")] [TableList(AlwaysExpanded = true, DrawScrollView = true)] [SerializeField]
        private List<PawnData> _pawns;

        [TabGroup("Schools")] [TableList(AlwaysExpanded = true, DrawScrollView = true)] [SerializeField]
        private List<School> _schools;

        // ────────────────────────────────  LIFECYCLE  ───────────────────────────────
        private new void OnEnable()
        {
            LoadAllData();
        }

        // ───────────────────────────────  WINDOW ENTRY  ─────────────────────────────
        [MenuItem("Tools/Gameplay Data Editor")]
        private static void Open()
        {
            GetWindow<GameplayDataEditor>("Gameplay Data");
        }

        // ────────────────────────────────  HELPERS  ─────────────────────────────────
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
            result.AddRange(guids.Select(guid => AssetDatabase.LoadAssetAtPath<T>(AssetDatabase.GUIDToAssetPath(guid)))
                .Where(asset => asset));
            return result;
        }
    }
}