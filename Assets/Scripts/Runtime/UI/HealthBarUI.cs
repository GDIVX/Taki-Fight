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
            _healthSystem.OnHealthChanged += (_, _) => UpdateHealth();
        }

        private void UpdateHealth()
        {
            var currHealth = _healthSystem.GetHealth();
            var maxHealth = _healthSystem.GetHealthMax();
            var ratio = currHealth / maxHealth;

            //update text
            _healthText.text = $"{currHealth}/{maxHealth}";

            //animate health bar
            var sequence = DOTween.Sequence();

            sequence.Append(_healthBarFillImage.DOFillAmount(ratio, .25f)).SetEase(Ease.InOutSine);
            sequence.AppendInterval(_trailDelay);
            sequence.Append(_healthBarFillImage.DOFillAmount(ratio, .3f)).SetEase(Ease.InOutSine);
            sequence.Play();
        }
    }
}