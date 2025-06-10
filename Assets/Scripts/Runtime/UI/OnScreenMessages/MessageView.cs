using DG.Tweening;
using TMPro;
using UnityEngine;

// For TextMeshPro support

namespace Runtime.UI.OnScreenMessages
{
    public class MessageView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _text;
        [SerializeField] private CanvasGroup _canvasGroup;

        private bool _isActive;

        public void SetMessage(string message, Color color, float duration)
        {
            _text.text = message;
            _text.color = color;
            _isActive = true;

            // Play fade-in animation
            FadeIn();

            // Schedule fade-out
            Invoke(nameof(FadeOut), duration);
        }

        private void FadeIn()
        {
            //Use DoTween to fade in
            _canvasGroup.DOFade(1, 1f);
        }

        private void FadeOut()
        {
            // Animation logic
            _canvasGroup.DOFade(0, 1f);
            _isActive = false;
        }

        public bool IsActive()
        {
            return _isActive;
        }
    }
}
