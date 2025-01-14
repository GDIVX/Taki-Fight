using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Runtime.UI.Tooltip
{
    public abstract class TooltipCallerBase : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        protected TooltipController CurrentTooltip;
        [SerializeField] protected Vector2 BaseOffset;

        protected TooltipPool TooltipPool => GameManager.Instance.TooltipPool;


        public virtual void OnPointerEnter(PointerEventData eventData)
        {
            // To be implemented by derived classes
        }

        public virtual void OnPointerExit(PointerEventData eventData)
        {
            if (CurrentTooltip != null)
            {
                TooltipPool.ReturnTooltip(CurrentTooltip);
                CurrentTooltip = null;
            }
        }

        protected void ShowTooltip(TooltipData data, Transform positionSource, RectTransform canvasRect, Vector2 offset)
        {
            if (data == null) return;
            if (CurrentTooltip == null)
            {
                CurrentTooltip = TooltipPool.GetTooltip();
            }

            CurrentTooltip.SetTooltip(data.Header, data.SecondHeader, data.Description, data.BackgroundColor,
                data.Icon);

            // Position the tooltip
            CurrentTooltip.PositionTooltip(positionSource, canvasRect, offset);
        }
    }
}