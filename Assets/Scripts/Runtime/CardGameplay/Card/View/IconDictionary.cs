using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Runtime.CardGameplay.Card.View
{
    [CreateAssetMenu(fileName = "IconDictionary", menuName = "Card/IconDictionary")]
    public class IconDictionary : ScriptableObject
    {
        [System.Serializable]
        public struct IconEntry
        {
            public string Placeholder; // e.g., "{icon:sword}"
            public Sprite Sprite;      // Direct reference to the Sprite
        }

        [SerializeField] private List<IconEntry> _iconEntries = new List<IconEntry>();
        private Dictionary<string, Sprite> _iconDictionary;

        private void OnValidate()
        {
            // Initialize the dictionary for quick lookups
            _iconDictionary = new Dictionary<string, Sprite>();
            foreach (var entry in _iconEntries.Where(entry => !string.IsNullOrEmpty(entry.Placeholder) && entry.Sprite != null))
            {
                _iconDictionary[entry.Placeholder] = entry.Sprite;
            }
        }

        /// <summary>
        /// Gets the sprite for the given placeholder, or null if not found.
        /// </summary>
        public Sprite GetSprite(string placeholder)
        {
            return _iconDictionary.GetValueOrDefault(placeholder);
        }

        /// <summary>
        /// Exposes the dictionary for debugging or advanced usage.
        /// </summary>
        public IReadOnlyDictionary<string, Sprite> IconMap => _iconDictionary;
    }
}