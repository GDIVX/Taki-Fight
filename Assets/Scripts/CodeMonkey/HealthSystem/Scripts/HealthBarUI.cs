using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Runtime.CodeMonkey.HealthSystem.Scripts
{
    /// <summary>
    /// Simple UI Health Bar, sets the Image fillAmount based on the linked HealthSystem
    /// Check the Demo scene for a usage example
    /// </summary>
    public class HealthBarUI : MonoBehaviour
    {
        [Tooltip(
            "Optional; Either assign a reference in the Editor (that implements IGetHealthSystem) or manually call SetHealthSystem()")]
        [SerializeField]
        private GameObject getHealthSystemGameObject;

        [Tooltip("Image to show the Health Bar, should be set as Fill, the script modifies fillAmount")]
        [SerializeField]
        private Image _colorImage, _fillImage, _trailFillImage;

        [SerializeField] private float _trailDelay, _trailFillTime, _animationTime;
        [SerializeField] private Ease _trailEase, _fillEase, _colorEase;


        private global::CodeMonkey.HealthSystemCM.HealthSystem healthSystem;


        /// <summary>
        ///     Clean up events when this Game Object is destroyed
        /// </summary>
        private void OnDestroy()
        {
            if (healthSystem == null) return;
            healthSystem.OnHealthChanged -= HealthSystem_OnHealthChanged;
        }


        // private void Start() {
        //     if (HealthSystem.TryGetHealthSystem(getHealthSystemGameObject, out HealthSystem healthSystem)) {
        //         SetHealthSystem(healthSystem);
        //     }
        // }

        /// <summary>
        /// Set the Health System for this Health Bar
        /// </summary>
        public void SetHealthSystem(global::CodeMonkey.HealthSystemCM.HealthSystem healthSystem)
        {
            if (this.healthSystem != null)
            {
                this.healthSystem.OnHealthChanged -= HealthSystem_OnHealthChanged;
            }

            this.healthSystem = healthSystem;

            UpdateHealthBar();

            healthSystem.OnHealthChanged += HealthSystem_OnHealthChanged;
        }

        /// <summary>
        /// Event fired from the Health System when Health Amount changes, update Health Bar
        /// </summary>
        private void HealthSystem_OnHealthChanged(object sender, EventArgs e)
        {
            UpdateHealthBar();
        }

        /// <summary>
        /// Update Health Bar using the Image fillAmount based on the current Health Amount
        /// </summary>
        private void UpdateHealthBar()
        {
            var healthNormalized = healthSystem.GetHealthNormalized();


            var sequence = DOTween.Sequence();
            sequence.Append(DOTween
                .To((x) => _fillImage.fillAmount = x, _fillImage.fillAmount, healthNormalized, _animationTime)
                .SetEase(_fillEase));
            sequence.AppendInterval(_trailDelay);
            sequence.Append(DOTween
                .To((x) => _trailFillImage.fillAmount = x, _fillImage.fillAmount, healthNormalized, _trailFillTime)
                .SetEase(_trailEase));
            sequence.Play();
        }
    }
}