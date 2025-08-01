﻿using System;
using System.Collections.Generic;
using System.IO;
using Runtime;
using Runtime.CardGameplay.Card;
using Runtime.CardGameplay.Card.CardBehaviour;
using Runtime.Combat.Pawn;
using Runtime.Combat.Pawn.AttackFeedback;
using Runtime.Combat.Pawn.AttackMod;
using Runtime.Combat.Tilemap;
using Sirenix.OdinInspector;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Runtime.Combat.Pawn
{
    [CreateAssetMenu(fileName = "Pawn Data", menuName = "Pawns/Regular", order = 0)]
    public class PawnData : ScriptableObject
    {
// ─── ROOT BOXES (needed for 2-level paths) ──────────────────


// ─── GENERAL ────────────────────────────────────────────────
        [BoxGroup("General")] [LabelWidth(80)] [SerializeField]
        private string _title;

        [BoxGroup("General")] [LabelWidth(80)] [SerializeField]
        private string _tribe;

        [BoxGroup("General")] [TextArea(2, 4)] [SerializeField]
        private string _description;

        [BoxGroup("General/View")]
        [PreviewField(alignment: ObjectFieldAlignment.Center, height: 200)]
        [HideLabel]
        [SerializeField]
        [Tooltip("Primary visual of the pawn.")]
        private Sprite _sprite;

        [BoxGroup("General/Ownership")] [LabelText("Owner")] [SerializeField]
        private PawnOwner _owner;


// ─── STATS (four subtabs) ───────────────────────────────────
        [TabGroup("Stats", "Health")] [LabelText("HP")] [SerializeField]
        private int _health;

        [TabGroup("Stats", "Health")] [LabelText("DEF")] [SerializeField]
        private int _defense;

        [TabGroup("Stats", "Combat")] [LabelText("DMG")] [SerializeField]
        private int _damage;

        [TabGroup("Stats", "Combat")] [LabelText("Hits/Turn")] [SerializeField]
        private int _attacks;

        [TabGroup("Stats", "Combat")] [LabelText("Range")] [SerializeField]
        private int _attackRange;

        [TabGroup("Stats", "Combat")] [LabelText("Damage Type")] [SerializeField] [SerializeReference] [Required]
        public IDamageHandler DamageType = new NormalDamageHandler();

        [TabGroup("Stats", "Movement")] [LabelText("Speed")] [SerializeField]
        private int _speed;

        [TabGroup("Stats", "Movement")] [LabelText("Agile?")] [SerializeField]
        private bool _isAgile;

        [TabGroup("Stats", "Size")] [LabelText("Tile Size")] [SerializeField]
        private Vector2Int _size = new(1, 1);


// ─── CALLBACKS (collapsed lists) ────────────────────────────
        [ListDrawerSettings(DefaultExpandedState = false)]
        [BoxGroup("Callbacks")]
        [BoxGroup("Callbacks/On Summon")]
        [ValidateInput("ValidateStrategies", "Strategies are invalid")]
        [SerializeField]
        private List<PawnStrategyData> _summonStrategies;

        [ListDrawerSettings(DefaultExpandedState = false)]
        [BoxGroup("Callbacks/On Turn Start")]
        [SerializeField]
        [ValidateInput("ValidateStrategies", "Strategies are invalid")]
        private List<PawnStrategyData> _onTurnStartStrategies;

        [ListDrawerSettings(DefaultExpandedState = false)]
        [BoxGroup("Callbacks/On Attack")]
        [SerializeField]
        [ValidateInput("ValidateStrategies", "Strategies are invalid")]
        private List<PawnStrategyData> _onAttackStrategies;

        [BoxGroup("Callbacks/On Attack")] [SerializeField]
        private AttackFeedbackStrategyData _attackFeedbackStrategy;

        [ListDrawerSettings(DefaultExpandedState = false)]
        [BoxGroup("Callbacks/On Hit")]
        [SerializeField]
        [ValidateInput("ValidateStrategies", "Strategies are invalid")]
        private List<PawnStrategyData> _onHitStrategies;

        [ListDrawerSettings(DefaultExpandedState = false)]
        [BoxGroup("Callbacks/On Move")]
        [SerializeField]
        [ValidateInput("ValidateStrategies", "Strategies are invalid")]
        private List<PawnStrategyData> _onMoveStrategies;

        [ListDrawerSettings(DefaultExpandedState = false)]
        [BoxGroup("Callbacks/Movement Abilities")]
        [SerializeField]
        [ValidateInput("ValidateStrategies", "Strategies are invalid")]
        private List<PawnStrategyData> _movementAbilities;

        [ListDrawerSettings(DefaultExpandedState = false)]
        [BoxGroup("Callbacks/On Damaged")]
        [SerializeField]
        [ValidateInput("ValidateStrategies", "Strategies are invalid")]
        private List<PawnStrategyData> _onDamagedStrategies;

        [ListDrawerSettings(DefaultExpandedState = false)]
        [BoxGroup("Callbacks/On Killed")]
        [SerializeField]
        [ValidateInput("ValidateStrategies", "Strategies are invalid")]
        private List<PawnStrategyData> _onKilledStrategies;


// ─── CARD TAB ───────────────────────────────────────────────
        [TabGroup("Card")] [Title("Summon Card")] [SerializeField]
        private CardData _summonCard;


        // ─────────────────────────────────────────────────────────

        // Exposed Properties
        public List<PawnStrategyData> OnSummonStrategies => _summonStrategies;
        public List<PawnStrategyData> OnTurnStartStrategies => _onTurnStartStrategies;
        public List<PawnStrategyData> OnAttackStrategies => _onAttackStrategies;
        public AttackFeedbackStrategyData AttackFeedbackStrategy => _attackFeedbackStrategy;
        public List<PawnStrategyData> OnHitStrategies => _onHitStrategies;
        public List<PawnStrategyData> OnMoveStrategies => _onMoveStrategies;
        public List<PawnStrategyData> MovementAbilities => _movementAbilities;
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

        public string Title
        {
            get => _title;
            set => _title = value;
        }

        public string Tribe
        {
            get => _tribe;
            set => _tribe = value;
        }


#if UNITY_EDITOR
        private void OnValidate()
        {
            if (_size.x <= 0 || _size.y <= 0)
            {
                Debug.LogError("Size dimensions must be greater than zero. Resetting to default (1, 1).");
                _size = new Vector2Int(1, 1);
            }

            if (_summonCard) _summonCard.Image = _sprite;

            var allStrategies = new List<PawnStrategyData>();
            allStrategies.AddRange(_onHitStrategies);
            allStrategies.AddRange(_onMoveStrategies);
            allStrategies.AddRange(_movementAbilities);
            allStrategies.AddRange(_onDamagedStrategies);
            allStrategies.AddRange(_onKilledStrategies);
            allStrategies.AddRange(_summonStrategies);
            allStrategies.AddRange(_onTurnStartStrategies);

            allStrategies.ForEach(data =>
            {
                if (!ValidateStrategy(data)) Debug.LogError($"Invalid strategy: {data} for {name}");
            });
        }
#endif

        public CardData CreateRuntimeSummonCard(int cost)
        {
            var card = CreateInstance<CardData>();

            var strategy = CreateInstance<SummonUnitPlay>();
            var parameters = new SummonUnitParams
            {
                Unit = this
            };

            var playStrategy = new PlayStrategyData
            {
                PlayStrategy = strategy,
                Potency = 1,
                Parameters = parameters
            };

            card.PlayStrategies = new List<PlayStrategyData> { playStrategy };

            return card;
        }

        [Button]
        public void WriteDescription()
        {
            DescriptionBuilder builder = new();
            _description = builder.AsSummon(this);
        }

        public void InitializeStrategies()
        {
            if (_attackFeedbackStrategy.Strategy != null)
                _attackFeedbackStrategy.Strategy.Initialize(_attackFeedbackStrategy);

            _onAttackStrategies.ForEach(data => data.Strategy.Initialize(data));
            _onHitStrategies.ForEach(data => data.Strategy.Initialize(data));
            _onDamagedStrategies.ForEach(data => data.Strategy.Initialize(data));
            _onKilledStrategies.ForEach(data => data.Strategy.Initialize(data));
            _onMoveStrategies.ForEach(data => data.Strategy.Initialize(data));
            _movementAbilities.ForEach(data => data.Strategy.Initialize(data));
            _onTurnStartStrategies.ForEach(data => data.Strategy.Initialize(data));
            _summonStrategies.ForEach(data => data.Strategy.Initialize(data));
        }

#if UNITY_EDITOR


        public bool ValidateStrategies(List<PawnStrategyData> strategies)
        {
            return strategies.TrueForAll(ValidateStrategy);
        }

        private static bool ValidateStrategy(PawnStrategyData strategy)
        {
            return strategy.Strategy;
        }


        [Button(ButtonSizes.Medium)]
        public void CreateSummonCard(int cost)
        {
            if (_summonCard) return;

            var card = CreateInstance<CardData>();
            card.Image = _sprite;

            var strategy = CreateInstance<SummonUnitPlay>();
            var parameters = new SummonUnitParams
            {
                Unit = this,
            };

            card.Title = _title;
            card.Cost = cost;
            card.Description = _description;
            card.CardType = CardType.Familiar;
            card.IsConsumed = true;
            var playStrategy = new PlayStrategyData
            {
                PlayStrategy = strategy,
                Potency = 1,
                Parameters = parameters
            };
            card.Highlight = HighlightType.Friendly;
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
#endif
    }
}

[Serializable]
public struct PawnStrategyData
{
    [Tooltip("The specific strategy applied to the pawn.")]
    public PawnPlayStrategy Strategy;

    [SerializeReference] public StrategyParams Parameters;

    [Tooltip("The potency or strength of the applied strategy.")]
    public int Potency;
}