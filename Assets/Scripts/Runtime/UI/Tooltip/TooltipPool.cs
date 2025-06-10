using System.Collections.Generic;
using UnityEngine;
using Utilities;

namespace Runtime.UI.Tooltip
{
    public class TooltipPool : MonoService<TooltipPool>
    {
        [SerializeField] private TooltipController _tooltipPrefab;
        [SerializeField] private int _initialSize = 5;

        private Queue<TooltipController> _pool = new Queue<TooltipController>();

        private void Awake()
        {
            // Pre-instantiate a pool of tooltips
            for (int i = 0; i < _initialSize; i++)
            {
                CreateNewTooltip();
            }
        }

        private TooltipController CreateNewTooltip()
        {
            var tooltip = Instantiate(_tooltipPrefab, transform);
            tooltip.gameObject.SetActive(false);
            _pool.Enqueue(tooltip);
            return tooltip;
        }

        public TooltipController GetTooltip()
        {
            // Reuse tooltip if available, otherwise create a new one
            if (_pool.Count > 0)
            {
                var tooltip = _pool.Dequeue();
                return tooltip;
            }

            return CreateNewTooltip();
        }

        public void ReturnTooltip(TooltipController tooltip)
        {
            tooltip.HideTooltip();
            _pool.Enqueue(tooltip);
        }
    }
}
