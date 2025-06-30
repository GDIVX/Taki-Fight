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

namespace Runtime.Combat.Pawn.VIew
{
    public class PawnView : MonoBehaviour
    {
        private static readonly int FlashAmount = Shader.PropertyToID("_FlashAmount");
        private static readonly int Dissolve = Shader.PropertyToID("_Dissolve");

        [SerializeField, Required] PawnFloatingTextManager _floatingTextManager;

        [SerializeField] [BoxGroup("Health")] private Image _defenseImage;

        [SerializeField] [BoxGroup("Health")] private TextMeshProUGUI _defenseCount;

        [SerializeField] [BoxGroup("Health")] private float _dissolveTime;

        [SerializeField] [BoxGroup("Health")] private Ease _dissolveEase;

        [SerializeField] [BoxGroup("Damage")] private float _flashTime;

        [SerializeField] [BoxGroup("Damage")] private Ease _flashEase;

        [SerializeField] [BoxGroup("Damage")] private TextMeshProUGUI _damageText;

        [SerializeField] [BoxGroup("Damage")] private TextMeshProUGUI _attacksText;

        [SerializeField] [BoxGroup("Damage")] private GameObject _multiStrikeObject;
        [SerializeField] [BoxGroup("Damage")] private RectTransform _gui;


        [SerializeField, BoxGroup("Movement")] private Ease _movementEase;
        [SerializeField, BoxGroup("Movement")] private float _movementDuration;

        [SerializeField] [BoxGroup("Health")] private HealthBarUI _healthBar;
        [SerializeField] [BoxGroup("Sprite")] private SpriteRenderer _spriteRenderer;

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
            _spriteRenderer ??= GetComponent<SpriteRenderer>();
            _floatingTextManager ??= GetComponent<PawnFloatingTextManager>();

            InitiateHealthView(controller);
            InitiateDefenseView(defense);

            InitiateAttackView(controller, data);

            _spriteRenderer.sprite = data.Sprite;

            _controller = controller;
            _controller.Combat.OnBeingAttacked += Flash;

            ApplyFootprintScale(data.Size.x, data.Size.y);
            ApplyOrientation(data.Owner);

            controller.Combat.OnBeingAttacked += (rawDamage, _) =>
            {
                _floatingTextManager.ShowDamageNumber(rawDamage , controller.Health.GetHealth());
            };
        }

        public bool IsAnimating()
        {
            return DOTween.IsTweening(_spriteRenderer);
        }

        private void InitiateAttackView(PawnController controller, PawnData data)
        {
            //damage
            var damage = controller.Combat.Damage;

            damage.OnValueChanged += UpdateDamageView;

            //attacks 
            var attacks = controller.Combat.Attacks;

            attacks.OnValueChanged += UpdateAttacksView;

            UpdateDamageView(damage.Value);
            UpdateAttacksView(attacks.Value);
        }

        private void UpdateAttacksView(int attacks)
        {
            if (attacks <= 1)
            {
                _multiStrikeObject.SetActive(false);
                _attacksText.text = "";
            }
            else
            {
                _multiStrikeObject.SetActive(true);
                _attacksText.text = $"X{attacks}";
            }
        }

        private void UpdateDamageView(int damage)
        {
            _damageText.text = damage.ToString();
        }

        private void ApplyOrientation(PawnOwner owner)
        {
            if (owner == PawnOwner.Enemy)
            {
                // Flip the sprite for enemy pawns
                _spriteRenderer.flipX = true;

                // Use the RadialLayout extension to flip the layout angles
                // _layout.Flip();
            }
            else
            {
                // Ensure the sprite is not flipped for friendly (Player or Castle) pawns
                _spriteRenderer.flipY = false;
            }

            // // Adjust the anchors and pivot based on ownership
            // if (!_layout.TryGetComponent(out RectTransform rectTransform)) return;
            // if (owner == PawnOwner.Enemy)
            // {
            //     rectTransform.anchorMin = new Vector2(1, 1);
            //     rectTransform.anchorMax = new Vector2(1, 1);
            //     rectTransform.pivot = new Vector2(1, 1);
            // }
            // else
            // {
            //     rectTransform.anchorMin = new Vector2(0, 1);
            //     rectTransform.anchorMax = new Vector2(0, 1);
            //     rectTransform.pivot = new Vector2(0, 1);
            // }
            //
            // rectTransform.anchoredPosition = Vector2.zero;
        }

        private void ApplyFootprintScale(int width, int height)
        {
            // Get tilemap information
            var tilemap = ServiceLocator.Get<TilemapController>();

            // Calculate the size of the footprint excluding padding
            var tileSize = tilemap.View.TileSize; // Size of each tile
            var totalWidth = width * tileSize;
            var totalHeight = height * tileSize;

            // Adjust scale of the pawn to fit footprint
            transform.localScale = new Vector3(
                totalWidth / _spriteRenderer.bounds.size.x,
                totalHeight / _spriteRenderer.bounds.size.y,
                1
            );
        }


        public void OnDead(Action onComplete)
        {
            if (!_spriteRenderer || !_spriteRenderer.gameObject.activeInHierarchy) return;

            DOTween.To(x => _spriteRenderer.material.SetFloat(Dissolve, x),
                0,
                1,
                _dissolveTime).SetEase(_dissolveEase).onComplete += () => { onComplete?.Invoke(); };
        }

        [Button]
        private void Flash(int attackPoints, int realDamage)
        {
            if (!_spriteRenderer || !_spriteRenderer.gameObject.activeInHierarchy)
                return;

            DOTween.To(
                    x =>
                    {
                        if (_spriteRenderer && _spriteRenderer.material)
                            _spriteRenderer.material.SetFloat(FlashAmount, x);
                    },
                    0f, 1f, _flashTime)
                .SetEase(_flashEase)
                .SetLoops(2, LoopType.Yoyo)
                .SetId("FlashTween_" + GetInstanceID())
                .OnComplete(() =>
                {
                    if (_spriteRenderer && _spriteRenderer.material)
                        _spriteRenderer.material.SetFloat(FlashAmount, 0f);
                });
        }


        private void InitiateDefenseView(Observable<int> defense)
        {
            _defense = defense;
            _defense.OnValueChanged += UpdateDefenseUI;
            UpdateDefenseUI(_defense.Value);
        }

        private void InitiateHealthView(PawnController controller)
        {
            _healthBar.SetUp(controller.Health);
        }

        private void UpdateDefenseUI(int defensePoints)
        {
            if (defensePoints <= 0)
            {
                _defenseImage.gameObject.SetActive(false);
                _defenseCount.text = "";
            }
            else
            {
                _defenseImage.gameObject.SetActive(true);
                _defenseCount.text = defensePoints.ToString();
            }
        }

        internal void SpawnAtPosition(Vector2Int anchor)
        {
            var targetPosition = CalculateCenterPosition(anchor);
            transform.position = targetPosition;
        }


        internal void MoveToPosition(Vector2Int anchor, Action onMoveComplete = null)
        {
            if (transform.SafeIsUnityNull()) return;

            var targetPosition = CalculateCenterPosition(anchor);

            transform.DOMove(targetPosition, _movementDuration)
                .SetEase(_movementEase)
                .OnComplete(() =>
                {
                    // Snap to the target position for safety
                    transform.position = targetPosition;

                    // Notify that movement is complete
                    onMoveComplete?.Invoke();
                });
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