using System;
using System.Collections;
using CodeMonkey.HealthSystemCM;
using DG.Tweening;
using Sirenix.OdinInspector;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.UI;
using Utilities;
using Runtime.Selection;
using UnityEngine.EventSystems;

namespace Runtime.Combat.Pawn
{
    public class PawnView : MonoBehaviour
    {
        [SerializeField, BoxGroup("Animation")]
        private Animator animator;

        [SerializeField, BoxGroup("Health")] private Image defenseImage;
        [SerializeField, BoxGroup("Health")] private TextMeshProUGUI defenseCount;
        [SerializeField, BoxGroup("Health")] private float _dissolveTime;
        [SerializeField, BoxGroup("Health")] private Ease _dissolveEase;
        [SerializeField, BoxGroup("Damage")] private float _flashTime;
        [SerializeField, BoxGroup("Damage")] private Ease _flashEase;

        [SerializeField, BoxGroup("Health")] private HealthBarUI healthBar;
        [SerializeField, BoxGroup("Health")] private HealthBarTextUI healthBarText;
        [SerializeField, BoxGroup("Sprite")] private SpriteRenderer spriteRenderer;

        [SerializeField, BoxGroup("Selection")]
        private Highlight highlightEffect; 

        private bool _showHighligh = false;

        private PawnController _controller;
        private TrackedProperty<int> _defense;
        private static readonly int FlashAmount = Shader.PropertyToID("_FlashAmount");
        private static readonly int Dissolve = Shader.PropertyToID("_Dissolve");

        public void Init(PawnController controller, TrackedProperty<int> defense, PawnData data)
        {
            InitiateHealthView(controller);
            InitiateDefenseView(defense);

            spriteRenderer.sprite = data.Sprite;

            _controller = controller;
            _controller.OnBeingAttacked += Flash;

            SelectionService.Instance.OnSearchInitialized += HandleSelectionHighlight;
        }


        public void OnDead(Action onComplete)
        {
            DOTween.To((x) => spriteRenderer.material.SetFloat(Dissolve, x),
                0,
                1,
                _dissolveTime).SetEase(_dissolveEase).onComplete += () => { onComplete?.Invoke(); };
        }

        [Button]
        private void Flash(int attackPoints, int realDamage)
        {
            DOTween.To(
                    x => spriteRenderer.material.SetFloat(FlashAmount, x),
                    0f, 1f, _flashTime)
                .SetEase(_flashEase)
                .SetLoops(2, LoopType.Yoyo)
                .SetId("FlashTween")
                .OnComplete(() => spriteRenderer.material.SetFloat(FlashAmount, 0f));
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
                _controller.OnBeingAttacked -= Flash;
            }

            if (_defense != null)
            {
                _defense.OnValueChanged -= UpdateDefenseUI;
            }
        }

        // =======================================
        //  SELECTION SERVICE INTEGRATION
        // =======================================

        private void HandleSelectionHighlight(Predicate<ISelectableEntity> predicate)
        {
            if (predicate.Invoke(_controller))
            {
                highlightEffect.Show();
            }
            else
            {
                highlightEffect.Hide();
            }
        }

        /// <summary>
        /// Called when the selection process is canceled or completed.
        /// </summary>
        public void ClearSelection()
        {
            highlightEffect.Hide();
        }

        /// <summary>
        /// Flash animation when the pawn is selected.
        /// </summary>
        public void OnSelected()
        {
            StartCoroutine(HideHighlightAfterFrame());
        }

        IEnumerator HideHighlightAfterFrame()
        {
            yield return new WaitForEndOfFrame();
            highlightEffect.Hide();
        }

        internal void SetPosition(Vector2Int position)
        {
            throw new NotImplementedException();
        }
    }
}