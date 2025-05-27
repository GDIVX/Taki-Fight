using System;
using System.Collections.Generic;
using System.IO;
using Runtime.CardGameplay.Card;
using Runtime.CardGameplay.Card.CardBehaviour;
using Runtime.Combat.Pawn;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace Runtime.Combat.Pawn
{
    [CreateAssetMenu(fileName = "Pawn Data", menuName = "Pawns/Regular", order = 0)]
    public class PawnData : ScriptableObject
    {
        [TabGroup("General", "Info")] [Title("Identity")] [SerializeField] [LabelWidth(80)]
        private string _title;

        [TabGroup("General", "Info")] [SerializeField] [TextArea]
        private string _description;

        [TabGroup("General", "View")]
        [PreviewField(100)]
        [HideLabel]
        [SerializeField]
        [Tooltip("The primary visual representation of the pawn.")]
        private Sprite _sprite;

        [TabGroup("General", "Info")]
        [Title("Ownership")]
        [SerializeField]
        [Tooltip("Specifies the pawn owner's identity.")]
        private PawnOwner _owner;

        [TabGroup("Stats", "Health")]
        [Title("Health")]
        [SerializeField]
        [Tooltip("The total health points of the pawn.")]
        private int _health;

        [SerializeField] [Tooltip("The defense value that reduces incoming damage.")]
        private int _defense;

        [TabGroup("Stats", "Combat")]
        [Title("Combat")]
        [SerializeField]
        [Tooltip("The amount of damage this pawn deals in combat.")]
        private int _damage;

        [TabGroup("Stats", "Combat")]
        [SerializeField]
        [Tooltip("The number of attacks this pawn can perform per turn.")]
        private int _attacks;

        [TabGroup("Stats", "Combat")] [SerializeField] [Tooltip("The attack range of the pawn in tiles.")]
        private int _attackRange;

        [TabGroup("Stats", "Movement")]
        [Title("Movement")]
        [SerializeField]
        [Tooltip("The movement speed of the pawn in tiles per turn.")]
        private int _speed;

        [SerializeField] [Tooltip("Indicates if this pawn is agile and may have agility-based abilities.")]
        private bool _isAgile;

        [TabGroup("Stats", "Size")] [Title("Size")] [SerializeField] [Tooltip("The pawn size in tiles.")]
        private Vector2Int _size = new(1, 1);

        [Title("On Summon")] [SerializeField] [Tooltip("Strategies triggered when the pawn is summoned.")]
        private List<PawnStrategyData> _summonStrategies;

        [Title("On Turn Start")] [SerializeField] [Tooltip("Strategies that execute when the turn starts.")]
        private List<PawnStrategyData> _onTurnStartStrategies;

        [Title("On Attack")] [SerializeField] [Tooltip("Strategies that execute when the pawn attacks.")]
        private List<PawnStrategyData> _onAttackStrategies;

        [Title("On Move")] [SerializeField] [Tooltip("Strategies triggered when the pawn moves.")]
        private List<PawnStrategyData> _onMoveStrategies;

        [Title("On Damaged")] [SerializeField] [Tooltip("Strategies triggered when the pawn takes damage.")]
        private List<PawnStrategyData> _onDamagedStrategies;

        [Title("On Killed")] [SerializeField] [Tooltip("Strategies triggered when the pawn is killed.")]
        private List<PawnStrategyData> _onKilledStrategies;

        [TabGroup("Card")] [Title("Summon Card")] [SerializeField]
        private CardData _summonCard;

        // Exposed Properties
        public List<PawnStrategyData> OnSummonStrategies => _summonStrategies;
        public List<PawnStrategyData> OnTurnStartStrategies => _onTurnStartStrategies;
        public List<PawnStrategyData> OnAttackStrategies => _onAttackStrategies;
        public List<PawnStrategyData> OnMoveStrategies => _onMoveStrategies;
        public List<PawnStrategyData> OnDamagedStrategies => _onDamagedStrategies;
        public List<PawnStrategyData> OnKilledStrategies => _onKilledStrategies;

        public Vector2Int Size
        {
            get => _size;
            set
            {
                if (value.x <= 0 || value.y <= 0)
                    throw new ArgumentException("Size dimensions must be greater than zero.");
                _size = value;
            }
        }

        public int Health => _health;
        public int Defense => _defense;

        public Sprite Sprite
        {
            get => _sprite;
            set => _sprite = value;
        }

        public bool IsAgile => _isAgile;
        public int Damage => _damage;
        public int Attacks => _attacks;
        public int Speed => _speed;
        public int AttackRange => _attackRange;
        public PawnOwner Owner => _owner;

        private void OnValidate()
        {
            if (_size.x <= 0 || _size.y <= 0)
            {
                Debug.LogError("Size dimensions must be greater than zero. Resetting to default (1, 1).");
                _size = new Vector2Int(1, 1);
            }

            if (_summonCard) _summonCard.Image = _sprite;
        }

        [Button(ButtonSizes.Medium)]
        public void CreateSummonCard()
        {
            if (_summonCard) return;

            var card = CreateInstance<CardData>();
            card.Image = _sprite;

            var strategy = CreateInstance<SummonUnitPlay>();
            strategy.Pawn = this;
            strategy.TileSelectionMode = TileSelectionMode.FriendlyEmpty;

            card.Title = _title;
            card.Description = _description;
            var playStrategy = new PlayStrategyData
            {
                PlayStrategy = strategy,
                Potency = 1
            };
            card.PlayStrategies = new List<PlayStrategyData> { playStrategy };

            if (!AssetDatabase.IsValidFolder("Assets/Resources/Data/Strategies/Summon"))
                Directory.CreateDirectory("Assets/Resources/Data/Strategies/Summon");

            if (!AssetDatabase.IsValidFolder("Assets/Resources/Data/Cards/Summon"))
                Directory.CreateDirectory("Assets/Resources/Data/Cards/Summon");

            AssetDatabase.CreateAsset(strategy, $"Assets/Resources/Data/Strategies/Summon/Summon_{_title}_Strat.asset");
            AssetDatabase.CreateAsset(card, $"Assets/Resources/Data/Cards/Summon/Summon_{_title}_Card.asset");
            AssetDatabase.SaveAssets();

            _summonCard = card;
        }
    }
}

[Serializable]
public struct PawnStrategyData
{
    [Tooltip("The specific strategy applied to the pawn.")]
    public PawnPlayStrategy Strategy;

    [Tooltip("The potency or strength of the applied strategy.")]
    public int Potency;
}