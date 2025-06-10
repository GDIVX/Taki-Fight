using System;
using Runtime.UI.Tooltip;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Runtime.Combat.StatusEffects
{
    public class StatusEffectView : MonoBehaviour
    {
        [SerializeField] private Image _image;
        [SerializeField] private TextMeshProUGUI _stackCount;
        [SerializeField] private TooltipCaller _tooltipCaller;

        private IStatusEffect _statusEffect;

        public void Init(IStatusEffect statusEffect, Sprite icon, TooltipData tooltipData)
        {
            _statusEffect = statusEffect;
            _image.sprite = icon;
            _tooltipCaller.SetData(tooltipData);
            UpdateStackText(statusEffect);

            statusEffect.Stack.OnValueChanged += UpdateStackText;
        }

        public void OnDestroy()
        {
            _statusEffect.Stack.OnValueChanged -= UpdateStackText;
        }

        private void UpdateStackText(int count)
        {
            _stackCount.text = count.ToString();
        }


        private void UpdateStackText(IStatusEffect statusEffect)
        {
            _stackCount.text = statusEffect.Stack.Value.ToString();
        }
    }
}
