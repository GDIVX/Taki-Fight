using System;
using CodeMonkey.HealthSystemCM;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace Runtime.UI
{
    public class HealthBarTextUI : MonoBehaviour
    {
        [SerializeField, Required] private TextMeshProUGUI text;
        [SerializeField] private IGetHealthSystem _getHealth;
        private HealthSystem _healthSystem;

        public void SetHealthSystem(HealthSystem healthSystem)
        {
            _healthSystem = healthSystem;
            _healthSystem.OnHealthChanged += OnHealthChanged;

            UpdateHealthText();
        }

        private void OnHealthChanged(object sender, EventArgs e)
        {
            UpdateHealthText();
        }

        private void UpdateHealthText()
        {
            text.text = _healthSystem.GetHealth().ToString();
        }
    }
}