﻿using System;
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
        [SerializeField, BoxGroup("Damage")] private Color _damageTakenColor;
        [SerializeField, BoxGroup("Damage")] private Color _damageBlockedColor;
        [SerializeField, BoxGroup("Damage")] private float _damageFadeAnimTime;
        [SerializeField, BoxGroup("Damage")] private Ease _damageFadeAnimEase;

        [SerializeField, BoxGroup("Health")] private HealthBarUI healthBar;
        [SerializeField, BoxGroup("Health")] private HealthBarTextUI healthBarText;
        [SerializeField, BoxGroup("Sprite")] private SpriteRenderer spriteRenderer;

        private PawnController _controller;
        private TrackedProperty<int> _defense;

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
            var color = realDamage > 0 ? _damageTakenColor : _damageBlockedColor;

            var tween = spriteRenderer.DOColor(color, _damageFadeAnimTime)
                .SetEase(_damageFadeAnimEase)
                .SetLoops(2, LoopType.Yoyo);
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