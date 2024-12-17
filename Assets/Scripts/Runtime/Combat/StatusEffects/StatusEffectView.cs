using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Runtime.Combat.StatusEffects
{
    public class StatusEffectView : MonoBehaviour
    {
        [SerializeField] private Image _image;
        [SerializeField] private TextMeshProUGUI _stackCount;

        private IStatusEffect _statusEffect;

        public void Init(IStatusEffect statusEffect, Sprite icon)
        {
            _statusEffect = statusEffect;
            _image.sprite = icon;
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