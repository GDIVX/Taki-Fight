using System;
using DG.Tweening;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace Runtime.UI
{
    public class BannerView : MonoBehaviour
    {
        [SerializeField, Required] private TextMeshProUGUI _textField;

        [ShowInInspector, ReadOnly] public bool IsShowingMessage { get; private set; } = true;

        public void SetMessage(string message, Color color, float fadeInDurationInSeconds)
        {
            _textField.color = color;
            _textField.text = message;
            _textField.DOFade(1, fadeInDurationInSeconds);
            IsShowingMessage = true;
        }

        public void Clear(float fadeOutDurationInSeconds)
        {
            _textField.text = "";
            _textField.DOFade(0, fadeOutDurationInSeconds);
            IsShowingMessage = false;
        }

        private void OnValidate()
        {
            _textField ??= GetComponentInChildren<TextMeshProUGUI>();
        }
    }
}