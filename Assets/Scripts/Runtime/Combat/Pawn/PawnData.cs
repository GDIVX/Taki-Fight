using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Runtime.Combat.Pawn
{
    [CreateAssetMenu(fileName = "Pawn Data", menuName = "Pawns/Regular", order = 0)]
    public class PawnData : ScriptableObject
    {
        [SerializeField] [Tooltip("The primary visual representation of the pawn.")] [TabGroup("View")]
        private Sprite _sprite;

        [SerializeField] [Tooltip("The total health points of the pawn.")] [BoxGroup("Health")]
        private int _health;

        [SerializeField] [Tooltip("The defense value that reduces incoming damage.")] [BoxGroup("Health")]
        private int _defense;

        [SerializeField] [Tooltip("The amount of damage this pawn deals in combat.")] [BoxGroup("Combat")]
        private int _damage;

        [SerializeField] [Tooltip("The number of attacks this pawn can perform per turn.")] [BoxGroup("Combat")]
        private int _attacks;

        [SerializeField] [Tooltip("The movement speed of the pawn in tiles per turn.")] [BoxGroup("Movement")]
        private int _speed;

        [SerializeField] [Tooltip("The attack range of the pawn in tiles.")] [BoxGroup("Combat")]
        private int _attackRange;

        [SerializeField] [Tooltip("Indicates if this pawn is agile and may have agility-based abilities.")]
        private bool _isAgile;

        [SerializeField] [Tooltip("Specifies the pawn owner's identity.")]
        private PawnOwner _owner;

        [SerializeField] [Tooltip("The pawn size in tiles.")] [BoxGroup("Size")]
        private Vector2Int _size = new(1, 1);

        [SerializeField] [Tooltip("Strategies that execute when the turn starts.")] [BoxGroup("Strategies")]
        private List<PawnStrategyData> _onTurnStartStrategies;

        [SerializeField] [Tooltip("Strategies that execute when the pawn attacks.")] [BoxGroup("Strategies")]
        private List<PawnStrategyData> _onAttackStrategies;

        [SerializeField] [Tooltip("Strategies triggered when the pawn moves.")] [BoxGroup("Strategies")]
        private List<PawnStrategyData> _onMoveStrategies;

        [SerializeField] [Tooltip("Strategies triggered when the pawn takes damage.")] [BoxGroup("Strategies")]
        private List<PawnStrategyData> _onDamagedStrategies;

        [SerializeField] [Tooltip("Strategies triggered when the pawn is killed.")] [BoxGroup("Strategies")]
        private List<PawnStrategyData> _onKilledStrategies;

        [SerializeField] [Tooltip("Strategies triggered when the pawn is summoned.")] [BoxGroup("Strategies")]
        private List<PawnStrategyData> _summonStrategies;

        public List<PawnStrategyData> OnSummonStrategies => _summonStrategies;

        public List<PawnStrategyData> OnTurnStartStrategies => _onTurnStartStrategies;
        public List<PawnStrategyData> OnAttackStrategies => _onAttackStrategies;
        public List<PawnStrategyData> OnMoveStrategies => _onMoveStrategies;
        public List<PawnStrategyData> OnDamagedStrategies => _onDamagedStrategies;
        public List<PawnStrategyData> OnKilledStrategies => _onKilledStrategies;

        // Size properties
        public Vector2Int Size
        {
            get => _size;
            set
            {
                if (value.x <= 0 || value.y <= 0)
                {
                    throw new ArgumentException("Size dimensions must be greater than zero.");
                }

                _size = value;
            }
        }

        public int Health => _health;
        public int Defense => _defense;
        public Sprite Sprite => _sprite;
        public bool IsAgile => _isAgile;
        public int Damage => _damage;
        public int Attacks => _attacks;

        // Movement properties
        public int Speed => _speed;

        // New property
        public int AttackRange => _attackRange;
        public PawnOwner Owner => _owner;

        private void OnValidate()
        {
            if (_size.x <= 0 || _size.y <= 0)
            {
                Debug.LogError("Size dimensions must be greater than zero. Resetting to default (1, 1).");
                _size = new Vector2Int(1, 1);
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
}