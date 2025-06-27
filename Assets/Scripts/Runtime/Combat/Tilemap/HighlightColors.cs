using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Utilities;

namespace Runtime.Combat.Tilemap
{
    [CreateAssetMenu(fileName = "Highlight Colors", menuName = "Game/Visuals/Tile Highlight Colors", order = 0)]
    public class HighlightColors : ScriptableService<HighlightColors>
    {
        [SerializeField, TableList] public List<HighlightColor> Colors;
    }

    [System.Serializable]
    public struct HighlightColor
    {
        public Color Color;
        public HighlightType Type;
    }
}