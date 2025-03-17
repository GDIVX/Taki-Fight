using System.Collections.Generic;
using Runtime.Combat.Pawn;
using UnityEngine;

namespace Runtime.Combat
{
    [CreateAssetMenu(fileName = "Combat", menuName = "Combat")]
    public class CombatConfig : ScriptableObject
    {
        [SerializeField] private List<PawnData> enemies;

        public PawnData[] Enemies => enemies.ToArray();
    }
}