using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Runtime.CardGameplay.ManaSystem
{
    [CreateAssetMenu(fileName = "Gems Settings", menuName = "Game/Settings/Gems", order = 0)]
    public class GemsSettings : ScriptableObject
    {
        [SerializeField] private List<GemDefinition> _gemDefinitions;

        private Dictionary<GemType, Sprite> _sprites;

        private void OnEnable()
        {
            _sprites = new Dictionary<GemType, Sprite>();
            _gemDefinitions.ForEach(def => _sprites[def.GemType] = def.Icon);
        }

        public Sprite GetIcon(GemType type)
        {
            return _sprites[type];
        }
    }

    [Serializable]
    public struct GemDefinition
    {
        public GemType GemType;
        public Sprite Icon;
    }
}