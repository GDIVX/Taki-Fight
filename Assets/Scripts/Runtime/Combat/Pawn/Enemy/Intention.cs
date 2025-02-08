using System;
using System.Collections.Generic;
using System.Linq;
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
        [SerializeField, TableList] private List<IntentionViewConfig> _viewConfigs;

        // Bob animation settings
        [SerializeField, BoxGroup("Bob Settings")] private float _bobAmplitude = 5f;
        [SerializeField, BoxGroup("Bob Settings")] private float _bobDuration = 1f;
        [SerializeField, BoxGroup("Bob Settings")] private Ease _bobEase = Ease.InOutSine;

        private Vector3 _originalPosition;
        private Tween _bobTween;

        private void Awake()
        {
            _originalPosition = transform.localPosition;
        }

        public void ShowIntention(IntentionType intentionType, int potency = 0, int repeats = 0)
        {
            if (potency < 0) potency = 0;
            var config = _viewConfigs.First(x => x.Type == intentionType);
            var sprite = GetSprite(config, potency);
            var text = GetText(intentionType, potency, repeats);

            Draw(sprite, text);
        }

        private Sprite GetSprite(IntentionViewConfig config, int potency)
        {
            var steps = Mathf.FloorToInt((float)potency / config.PotencyStepSize);
            steps = Mathf.Clamp(steps, 0, config.Sprites.Count - 1);
            return config.Sprites[steps];
        }

        private string GetText(IntentionType intentionType, int potency, int repeats)
        {
            switch (intentionType)
            {
                case IntentionType.Attack:
                    return repeats > 1 ? $"{potency}X{repeats}" : potency.ToString();
                case IntentionType.Defense:
                    return potency.ToString();
                default:
                    return "";
            }
        }

        private void Draw(Sprite sprite, string text)
        {
            _image.sprite = sprite;
            _text.text = text;

            _image.DOFade(1, _animDuration).SetEase(_ease);
            _text.DOFade(1, _animDuration).SetEase(_ease);

            StartBobAnimation();
        }

        private void StartBobAnimation()
        {
            // Kill any existing bob tween before starting a new one.
            if (_bobTween != null) _bobTween.Kill();

            _bobTween = transform.DOLocalMoveY(_originalPosition.y + _bobAmplitude, _bobDuration)
                .SetEase(_bobEase)
                .SetLoops(-1, LoopType.Yoyo);
        }

        public void Hide()
        {
            _image.DOFade(0, _animDuration).SetEase(_ease);
            _text.DOFade(0, _animDuration).SetEase(_ease);

            // Stop the bob animation
            if (_bobTween != null)
            {
                _bobTween.Kill();
                _bobTween = null;
            }

            // Reset the position to the original one.
            transform.localPosition = _originalPosition;
        }

        [System.Serializable]
        public struct IntentionViewConfig
        {
            public IntentionType Type;
            public List<Sprite> Sprites;
            public int PotencyStepSize;
        }
    }
}
