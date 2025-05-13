using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using Utilities;

namespace Runtime.UI.Tooltip
{
    [CreateAssetMenu(fileName = "Tooltip Dictionary", menuName = "Game/Tooltip Dictionary", order = 0)]
    public class KeywordDictionary : ScriptableService<KeywordDictionary>
    {
        [SerializeField, AssetList] private List<TooltipData> _tooltipData;

        private Dictionary<string, TooltipData> _dictionary;
        [ShowInInspector, ReadOnly] public List<string> Keywords => _dictionary.Keys.ToList();

        private void OnEnable()
        {
            Initialize();
        }

        private void OnValidate()
        {
            Initialize();
        }

        private void Initialize()
        {
            _dictionary = new Dictionary<string, TooltipData>();

            foreach (TooltipData data in _tooltipData)
            {
                _dictionary[data.Header] = data;
            }
        }

        public TooltipData GetTooltip(string key)
        {
            return _dictionary.GetValueOrDefault(key);
        }
    }
}