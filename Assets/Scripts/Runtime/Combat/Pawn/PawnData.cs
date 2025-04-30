using Runtime.Combat.Tilemap;
using Sirenix.OdinInspector;
using System.Collections.Generic;
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
        [SerializeField, BoxGroup("Movement")] private bool _isFlyer; // Can the unit fly?
        [SerializeField, BoxGroup("Movement")] private Vector2Int _direction; // Movement direction
        [SerializeField, BoxGroup("Combat")] private List<Vector2Int> _attackRange; // New: Attack range
        [SerializeField] private bool _isAgile;
        [SerializeField] private PawnOwner _owner;

        public int Health => _health;
        public int Defense => _defense;
        public Sprite Sprite => _sprite;
        public bool IsAgile => _isAgile;
        public int Damage => _damage;
        public int Attacks => _attacks;

        // Movement properties
        public int Speed => _speed;
        public bool IsFlyer => _isFlyer;
        public Vector2Int Direction => _direction;

        // New property
        public List<Vector2Int> AttackRange => _attackRange;
        public PawnOwner Owner => _owner;
    }
}