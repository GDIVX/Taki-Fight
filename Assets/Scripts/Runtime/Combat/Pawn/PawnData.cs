using Sirenix.OdinInspector;
using UnityEngine;

namespace Runtime.Combat.Pawn
{
    [CreateAssetMenu(fileName = "Pawn Data", menuName = "Pawns/Regular", order = 0)]
    public class PawnData : ScriptableObject
    {
        [SerializeField, TabGroup("View")] private Sprite sprite;
        [SerializeField, TabGroup("Health")] private int health;
        [SerializeField, TabGroup("Health")] private int defense;

        public int Health => health;
        public int Defense => defense;
        public Sprite Sprite => sprite;
    }
}