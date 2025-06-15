using System.Collections.Generic;
using UnityEngine;

namespace Runtime.UI.Tooltip
{
    public class CompoundTooltipController : TooltipController
    {
        [SerializeField]
        private TooltipController _sectionPrefab;
        [SerializeField]
        private Transform _contentRoot;

        private readonly List<TooltipController> _sections = new();

        public void SetTooltip(CompoundTooltipData data)
        {
            foreach (var section in _sections)
            {
                if (section)
                {
                    Destroy(section.gameObject);
                }
            }
            _sections.Clear();

            if (data == null) return;

            foreach (var tooltip in data.Tooltips)
            {
                var section = Instantiate(_sectionPrefab, _contentRoot);
                section.SetTooltip(tooltip.Header, tooltip.SecondHeader,
                    tooltip.Description, tooltip.BackgroundColor, tooltip.Icon);
                _sections.Add(section);
            }
        }
    }
}
