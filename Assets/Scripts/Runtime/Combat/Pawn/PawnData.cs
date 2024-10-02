using UnityEngine;

namespace Runtime.Combat.Pawn
{
    [CreateAssetMenu(fileName = "Pawn Data", menuName = "Pawn", order = 0)]
    public class PawnData : ScriptableObject
    {
        [SerializeField] private int health;
        [SerializeField] private int defense;

        public int Health => health;
        public int Defense => defense;
    }
}