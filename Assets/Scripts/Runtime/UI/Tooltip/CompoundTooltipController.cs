using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.Utilities;
using UnityEngine;

namespace Runtime.UI.Tooltip
{
    public class CompoundTooltipController : TooltipController
    {
        [SerializeField] NestedSimpleTooltip _tooltipPrefab;

        private readonly Stack<NestedSimpleTooltip> _activeTooltips = new();
        private readonly Stack<NestedSimpleTooltip> _disableTooltips = new();


        public override void SetTooltip(ITooltipSource source)
        {
            if (source is CompoundTooltipSource compoundTooltipSource)
            {
                foreach (var t in compoundTooltipSource.TooltipSources)
                {
                    SetContentTooltip(t);
                }
            }

            if (source is not IContentTooltipSource tooltipSource) return;
            SetContentTooltip(tooltipSource);
        }

        private void SetContentTooltip(IContentTooltipSource tooltipSource)
        {
            var tooltip = GetTooltip();
            tooltip.SetTooltip(tooltipSource);
        }

        public NestedSimpleTooltip CreateNestedTooltip()
        {
            var tooltip = Instantiate(_tooltipPrefab, transform);
            _activeTooltips.Push(tooltip);
            return tooltip;
        }


        private NestedSimpleTooltip GetTooltip()
        {
            if (_disableTooltips.Count > 0) return _disableTooltips.Pop();

            var tooltip = CreateNestedTooltip();
            return tooltip;
        }


        public override void ShowTooltip()
        {
            CalculatePosition();
            gameObject.SetActive(true);
            _activeTooltips.ForEach(t => t.ShowTooltip());
        }

        public override void HideTooltip()
        {
            for (int i = 0; i < _activeTooltips.Count; i++)
            {
                var tooltip = _activeTooltips.Pop();
                _disableTooltips.Push(tooltip);
                tooltip.HideTooltip();
            }

            gameObject.SetActive(false);
        }

        public override void Reset()
        {
            for (int i = 0; i < _activeTooltips.Count; i++)
            {
                var tooltip = _activeTooltips.Pop();
                _disableTooltips.Push(tooltip);
                tooltip.Reset();
            }

            gameObject.SetActive(false);
        }
    }

    public struct CompoundTooltipSource : ITooltipSource
    {
        public List<IContentTooltipSource> TooltipSources { get; set; }
    }
}