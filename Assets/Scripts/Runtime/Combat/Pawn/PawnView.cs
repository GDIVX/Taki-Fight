using System;
using System.Linq;
using DG.Tweening;
using Runtime.Combat.Tilemap;
using Runtime.UI;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Utilities;
using HealthBarUI = Runtime.CodeMonkey.HealthSystem.Scripts.HealthBarUI;

namespace Runtime.Combat.Pawn
{
    public class PawnView : MonoBehaviour
    {
        private static readonly int FlashAmount = Shader.PropertyToID("_FlashAmount");
        private static readonly int Dissolve = Shader.PropertyToID("_Dissolve");

        [SerializeField, BoxGroup("Animation")]
        private Animator animator;

        [SerializeField, BoxGroup("Health")] private Image defenseImage;
        [SerializeField, BoxGroup("Health")] private TextMeshProUGUI defenseCount;
        [SerializeField, BoxGroup("Health")] private float _dissolveTime;
        [SerializeField, BoxGroup("Health")] private Ease _dissolveEase;
        [SerializeField, BoxGroup("Damage")] private float _flashTime;
        [SerializeField, BoxGroup("Damage")] private Ease _flashEase;
        [SerializeField, BoxGroup("Movement")] private Ease _movementEase;
        [SerializeField, BoxGroup("Movement")] private float _movementDuration;

        [SerializeField, BoxGroup("Health")] private HealthBarUI healthBar;
        [SerializeField, BoxGroup("Health")] private HealthBarTextUI healthBarText;
        [SerializeField, BoxGroup("Sprite")] private SpriteRenderer spriteRenderer;

        private PawnController _controller;
        private Observable<int> _defense;

        private void OnDestroy()
        {
            if (_controller != null) _controller.Combat.OnBeingAttacked -= Flash;

            if (_defense != null) _defense.OnValueChanged -= UpdateDefenseUI;

            // Kill any active DOTween animations to prevent null reference exceptions
            DOTween.Kill("FlashTween_" + GetInstanceID());
        }

        public void Init(PawnController controller, Observable<int> defense, PawnData data)
        {
            InitiateHealthView(controller);
            InitiateDefenseView(defense);

            spriteRenderer.sprite = data.Sprite;

            _controller = controller;
            _controller.Combat.OnBeingAttacked += Flash;

            ApplyFootprintScale(data.Size.x, data.Size.y);
        }

        private void ApplyFootprintScale(int width, int height)
        {
            //var tilemapView = ServiceLocator.Get<TilemapController>().View;
            float baseUnitSize = 1f;
            transform.localScale = new Vector3(width * baseUnitSize, height * baseUnitSize, 1f);
        }


        public void OnDead(Action onComplete)
        {
            if (!spriteRenderer || !spriteRenderer.gameObject.activeInHierarchy) return;

            DOTween.To((x) => spriteRenderer.material.SetFloat(Dissolve, x),
                0,
                1,
                _dissolveTime).SetEase(_dissolveEase).onComplete += () => { onComplete?.Invoke(); };
        }

        [Button]
        private void Flash(int attackPoints, int realDamage)
        {
            if (!spriteRenderer || !spriteRenderer.gameObject.activeInHierarchy)
                return;

            DOTween.To(
                    x =>
                    {
                        if (spriteRenderer != null && spriteRenderer.material != null)
                            spriteRenderer.material.SetFloat(FlashAmount, x);
                    },
                    0f, 1f, _flashTime)
                .SetEase(_flashEase)
                .SetLoops(2, LoopType.Yoyo)
                .SetId("FlashTween_" + GetInstanceID())
                .OnComplete(() =>
                {
                    if (spriteRenderer != null && spriteRenderer.material != null)
                        spriteRenderer.material.SetFloat(FlashAmount, 0f);
                });
        }


        private void InitiateDefenseView(Observable<int> defense)
        {
            // _defense = defense;
            // _defense.OnValueChanged += UpdateDefenseUI;
            // UpdateDefenseUI(_defense.Value);
        }

        private void InitiateHealthView(PawnController controller)
        {
            // var healthSystem = controller.Health;
            // // healthBar.SetHealthSystem(healthSystem);
            // healthBarText.SetHealthSystem(healthSystem);
        }

        private void UpdateDefenseUI(int defensePoints)
        {
            // defenseImage.gameObject.SetActive(defensePoints != 0);
            // defenseCount.text = defensePoints.ToString();
        }

        internal void SpawnAtPosition(Vector2Int anchor)
        {
            var targetPosition = CalculateCenterPosition(anchor);
            transform.position = targetPosition;
        }


        internal void MoveToPosition(Vector2Int anchor)
        {
            if (transform.SafeIsUnityNull()) return;

            var targetPosition = CalculateCenterPosition(anchor);

            transform.DOMove(targetPosition, _movementDuration)
                .SetEase(_movementEase)
                .OnComplete(() => transform.position = targetPosition);
        }

        private Vector3 CalculateCenterPosition(Vector2Int anchor)
        {
            var tilemap = ServiceLocator.Get<TilemapController>();
            var footprint = tilemap.GenerateFootprintUnbounded(anchor, _controller.TilemapHelper.Size);

            if (footprint.Length == 0)
            {
                tilemap.View.WorldToMapPoint(anchor);
            }

            var sum = footprint.Aggregate(Vector2.zero,
                (current, tile) => current + tilemap.View.MapToWorldPoint(tile.Position));

            return sum / footprint.Length;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            //Show attack range 
            var tilesInAttackRange = _controller.Combat.GetTilesInAttackRange();
            foreach (var tile in tilesInAttackRange)
            {
                var pawn = tile.Pawn;
                if (!pawn)
                {
                    tile.View.Highlight(Color.yellow);
                    continue;
                }

                if (pawn.Owner == _controller.Owner)
                {
                    tile.View.Highlight(Color.green);
                    continue;
                }

                tile.View.Highlight(Color.red);
            }
        }
    }
}