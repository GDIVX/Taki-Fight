using System;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using Utilities;

namespace Runtime.UI.Tooltip
{
    public abstract class TooltipCallerBase : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        protected TooltipController CurrentTooltip;
        [SerializeField] private float _delayTime;

        protected TooltipPool TooltipPool { get; private set; }

        private void Start()
        {
            TooltipPool = ServiceLocator.Get<TooltipPool>();
        }


        public virtual void OnPointerEnter(PointerEventData eventData)
        {
            // To be implemented by derived classes
        }

        public virtual void OnPointerExit(PointerEventData eventData)
        {
            if (CurrentTooltip == null) return;

            StopAllCoroutines();
            TooltipPool.ReturnTooltip(CurrentTooltip);
            CurrentTooltip = null;
        }

        protected void ShowTooltip(TooltipData data)
        {
            if (data == null) return;
            if (CurrentTooltip == null)
            {
                CurrentTooltip = TooltipPool.GetTooltip();
            }

            CurrentTooltip.SetTooltip(data.Header, data.SecondHeader, data.Description, data.BackgroundColor,
                data.Icon);
            this.Timer(_delayTime, () =>
            {
                CurrentTooltip.ShowTooltip();
            });
        }
    }
}
