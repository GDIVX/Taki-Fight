using System.Collections.Generic;
using UnityEngine;

namespace Runtime.UI.Tooltip
{
    public class CompoundTooltipData : ScriptableObject
    {
        [SerializeField]
        private List<TooltipData> _tooltips = new();

        public IReadOnlyList<TooltipData> Tooltips => _tooltips;

        public void SetTooltips(IEnumerable<TooltipData> tooltips)
        {
            _tooltips = new List<TooltipData>(tooltips);
        }

        public void AddTooltip(TooltipData data)
        {
            if (data == null) return;
            _tooltips.Add(data);
        }
    }
}
