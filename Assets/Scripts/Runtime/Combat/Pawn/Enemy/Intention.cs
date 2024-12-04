using DG.Tweening;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Runtime.Combat.Pawn.Enemy
{
    public class Intention : MonoBehaviour
    {
        [SerializeField] private Image _image;
        [SerializeField] private TextMeshProUGUI _text;
        [SerializeField, BoxGroup("settings")] private float _animDuration;
        [SerializeField, BoxGroup("settings")] private Ease _ease;


        public void Draw(Sprite sprite, Color color, string text)
        {
            _image.sprite = sprite;
            _image.color = new Color(color.r, color.g, color.b, 0f);
            _text.text = text;

            _image.DOFade(1, _animDuration).SetEase(_ease);
            _text.DOFade(1, _animDuration).SetEase(_ease);
        }

        public void Hide()
        {
            _image.DOFade(0, _animDuration).SetEase(_ease);
            _text.DOFade(0, _animDuration).SetEase(_ease);
        }
    }
}