using System;
using CodeMonkey.HealthSystemCM;
using DG.Tweening;
using Sirenix.OdinInspector;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.UI;
using Utilities;

namespace Runtime.Combat.Pawn
{
    public class PawnView : MonoBehaviour
    {
        [SerializeField, BoxGroup("Animation")]
        private Animator animator;

        [SerializeField, BoxGroup("Health")] private Image defenseImage;
        [SerializeField, BoxGroup("Health")] private TextMeshProUGUI defenseCount;
        [SerializeField, BoxGroup("Damage")] private float _flashTime;
        [SerializeField, BoxGroup("Damage")] private Ease _flashEase;

        [SerializeField, BoxGroup("Health")] private HealthBarUI healthBar;
        [SerializeField, BoxGroup("Health")] private HealthBarTextUI healthBarText;
        [SerializeField, BoxGroup("Sprite")] private SpriteRenderer spriteRenderer;

        private PawnController _controller;
        private TrackedProperty<int> _defense;
        private static readonly int IsFlashing = Shader.PropertyToID("_IsFlashing");
        private static readonly int FlashColor = Shader.PropertyToID("_FlashColor");
        private static readonly int FlashAmount = Shader.PropertyToID("_FlashAmount");

        public void Init(PawnController controller, TrackedProperty<int> defense, PawnData data)
        {
            InitiateHealthView(controller);
            InitiateDefenseView(defense);

            spriteRenderer.sprite = data.Sprite;

            _controller = controller;
            _controller.OnBeingAttacked += OnPawnBeingAttacked;
        }

        [Button]
        private void OnPawnBeingAttacked(int attackPoints, int realDamage)
        {
            DOTween.To((x) => spriteRenderer.material.SetFloat(FlashAmount, x),
                0f,
                1,
                _flashTime).SetEase(_flashEase).SetLoops(2, LoopType.Yoyo);
        }


        private void InitiateDefenseView(TrackedProperty<int> defense)
        {
            _defense = defense;
            _defense.OnValueChanged += UpdateDefenseUI;
            UpdateDefenseUI(_defense.Value);
        }

        private void InitiateHealthView(PawnController controller)
        {
            var healthSystem = controller.Health;
            healthBar.SetHealthSystem(healthSystem);
            healthBarText.SetHealthSystem(healthSystem);
        }


        private void UpdateDefenseUI(int defensePoints)
        {
            defenseImage.gameObject.SetActive(defensePoints != 0);
            defenseCount.text = defensePoints.ToString();
        }

        private void OnDestroy()
        {
            if (_controller != null)
            {
                _controller.OnBeingAttacked -= OnPawnBeingAttacked;
            }

            if (_defense != null)
            {
                _defense.OnValueChanged -= UpdateDefenseUI;
            }
        }
    }
}