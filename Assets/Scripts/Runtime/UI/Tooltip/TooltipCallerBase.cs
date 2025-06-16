using System;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using Utilities;

namespace Runtime.UI.Tooltip
{
    public abstract class TooltipCallerBase<T> : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
        where T : ITooltipSource
    {
        protected TooltipController CurrentTooltip;
        [SerializeField] private float _delayTime;
        [SerializeField] private TooltipController _prefab;

        protected TooltipPool TooltipPool => ServiceLocator.Get<TooltipPool>();

        private void Awake()
        {
            TooltipPool.Populate<T>(_prefab);
        }


        public virtual void OnPointerEnter(PointerEventData eventData)
        {
            // To be implemented by derived classes
        }

        public virtual void OnPointerExit(PointerEventData eventData)
        {
            if (CurrentTooltip == null) return;

            StopAllCoroutines();
            TooltipPool.ReturnTooltip<T>(CurrentTooltip);
            CurrentTooltip = null;
        }

        protected virtual void ShowTooltip(ITooltipSource source)
        {
            if (source == null)
            {
                Debug.LogError("Tooltip source cannot be null!");
                return;
            }

            if (!CurrentTooltip)
            {
                CurrentTooltip = TooltipPool.GetTooltip<T>();
            }


            CurrentTooltip.SetTooltip(source);
            this.Timer(_delayTime, () => { CurrentTooltip.ShowTooltip(); });
        }
    }
}