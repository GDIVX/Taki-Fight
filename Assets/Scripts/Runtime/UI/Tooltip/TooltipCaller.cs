using UnityEngine;
using UnityEngine.EventSystems;

namespace Runtime.UI.Tooltip
{
    public class TooltipCaller : TooltipCallerBase
    {
        [SerializeField] private TooltipData _tooltipData;

        public void SetData(TooltipData data)
        {
            _tooltipData = data;
        }

        public override void OnPointerEnter(PointerEventData eventData)
        {
            RectTransform canvasRect = GetComponentInParent<Canvas>().GetComponent<RectTransform>();
            ShowTooltip(_tooltipData, transform, canvasRect, BaseOffset); // Adjust offset as needed
        }
    }
}