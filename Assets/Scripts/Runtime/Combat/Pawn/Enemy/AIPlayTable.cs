using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;
using Utilities;

namespace Runtime.Combat.Pawn.Enemy
{
    [CreateAssetMenu(fileName = "Play Table", menuName = "AI/Play Table", order = 0)]
    public class AIPlayTable : ScriptableObject
    {
        [SerializeField , TableList] private List<PlayTableEntry> entries;

        public PlayTableEntry ChoseRandomPlayStrategy()
        {
            if (!entries.IsNullOrEmpty()) return entries.WeightedSelectRandom(entry => entry.Weight);
            Debug.LogError($"{name} has no entries for the play table.");
            return default;

        }
    }
}