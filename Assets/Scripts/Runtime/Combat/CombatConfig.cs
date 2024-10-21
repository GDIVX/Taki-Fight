using System.Collections.Generic;
using Runtime.Combat.Pawn;
using UnityEngine;

namespace Runtime.Combat
{
    [CreateAssetMenu(fileName = "Combat", menuName = "Combat")]
    public class CombatConfig : ScriptableObject
    {
        [SerializeField] private List<PawnController> enemies;

        public PawnController[] Enemies => enemies.ToArray();
    }
}