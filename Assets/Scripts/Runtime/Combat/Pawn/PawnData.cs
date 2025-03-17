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
        [SerializeField] private bool _isAgile;

        public int Health => _health;
        public int Defense => _defense;
        public Sprite Sprite => _sprite;
        public bool IsAgile => _isAgile;

        public int Damage => _damage;

        public int Attacks => _attacks;
    }
}