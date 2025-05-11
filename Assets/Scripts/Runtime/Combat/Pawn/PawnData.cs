using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Runtime.Combat.Pawn
{
    [CreateAssetMenu(fileName = "Pawn Data", menuName = "Pawns/Regular", order = 0)]
    public class PawnData : ScriptableObject
    {
        [SerializeField, TabGroup("View")] private Sprite _sprite;
        [SerializeField, BoxGroup("Health")] private int _health;
        [SerializeField, BoxGroup("Health")] private int _defense;
        [SerializeField, BoxGroup("Combat")] private int _damage;
        [SerializeField, BoxGroup("Combat")] private int _attacks;
        [SerializeField, BoxGroup("Movement")] private int _speed; // Base speed
        [SerializeField, BoxGroup("Movement")] private Vector2Int _direction; // Movement direction
        [SerializeField, BoxGroup("Combat")] private List<Vector2Int> _attackRange; // New: Attack range
        [SerializeField] private bool _isAgile;
        [SerializeField] private PawnOwner _owner;

        [SerializeField, BoxGroup("Size")] private Vector2Int _size = new Vector2Int(1, 1); // Size of the pawn in tiles

        [SerializeField] [BoxGroup("Strategies")]
        private List<PawnStrategyData> _onTurnStartStrategies;

        [SerializeField] [BoxGroup("Strategies")]
        private List<PawnStrategyData> _onAttackStrategies;

        [SerializeField] [BoxGroup("Strategies")]
        private List<PawnStrategyData> _onMoveStrategies;

        [SerializeField] [BoxGroup("Strategies")]
        private List<PawnStrategyData> _onDamagedStrategies;

        [SerializeField] [BoxGroup("Strategies")]
        private List<PawnStrategyData> _onKilledStrategies;

        [SerializeField] [BoxGroup("Strategies")]
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
        public Vector2Int Direction => _direction;

        // New property
        public List<Vector2Int> AttackRange => _attackRange;
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
        public PawnPlayStrategy Strategy;
        public int Potency;
    }
}