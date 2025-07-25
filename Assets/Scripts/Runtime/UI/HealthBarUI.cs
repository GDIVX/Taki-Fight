﻿using System;
using CodeMonkey.HealthSystemCM;
using DG.Tweening;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Runtime.UI
{
    public class HealthBarUI : MonoBehaviour
    {
        [SerializeField] [BoxGroup("HealthBar")] [Required]
        private Image _healthBarFillImage;

        [SerializeField] [BoxGroup("HealthBar")] [Required]
        private Image _healthBarTrailImage;

        [SerializeField] [BoxGroup("HealthBar")] [Required]
        private float _trailDelay = .4f;

        [SerializeField] [BoxGroup("Text")] [Required]
        private TextMeshProUGUI _healthText;

        [ShowInInspector] private HealthSystem _healthSystem;

        public void SetUp(HealthSystem healthSystem)
        {
            _healthBarFillImage.fillAmount = 1;
            _healthBarTrailImage.fillAmount = 1;

            _healthSystem = healthSystem;
            _healthSystem.OnHealthChanged += UpdateHealth;
            _healthSystem.OnDead += (sender, args) => _healthSystem.OnHealthChanged -= UpdateHealth;
            UpdateHealth();
        }

        private void OnDestroy()
        {
            _healthSystem.OnHealthChanged -= UpdateHealth;
        }

        private void UpdateHealth(object sender, EventArgs e)
        {
            UpdateHealth();
        }

        private void UpdateHealth()
        {
            if (!gameObject.activeInHierarchy) return;
            if (_healthSystem == null || _healthBarFillImage == null || _healthText == null)
            {
                Debug.LogError("HealthBarUI is not properly configured.");
                return;
            }

            var currHealth = _healthSystem.GetHealth();
            var maxHealth = _healthSystem.GetHealthMax();

            if (maxHealth <= 0)
            {
                Debug.LogWarning("Max health cannot be zero or negative.");
                _healthBarFillImage.fillAmount = 0;
                _healthText.text = "0/0";
                return;
            }

            var ratio = currHealth / maxHealth;

            // Update text
            _healthText.text = $"{Mathf.RoundToInt(currHealth)}/{Mathf.RoundToInt(maxHealth)}";

            // Animate health bar
            DOTween.Kill(_healthBarFillImage); // Prevent overlapping sequences
            _healthBarFillImage.fillAmount = ratio; // Instant update for health bar front
            var sequence = DOTween.Sequence();
            sequence.Append(_healthBarTrailImage.DOFillAmount(ratio, _trailDelay).SetEase(Ease.InOutSine));
            sequence.Play();
        }
    }
}