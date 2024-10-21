using System.Collections.Generic;
using Runtime.CardGameplay.Card.CardBehaviour;
using Sirenix.Utilities;
using UnityEngine;
using Utilities;

namespace Runtime.Combat.Pawn.Enemy
{
    [CreateAssetMenu(fileName = "Play Table", menuName = "AI/Play Table", order = 0)]
    public class AIPlayTable : ScriptableObject
    {
        [SerializeField] private List<PlayTableEntry> entries;

        public CardPlayStrategy ChoseRandomPlayStrategy()
        {
            if (entries.IsNullOrEmpty())
            {
                Debug.LogError($"{name} has no entries for the play table.");
                return default;
            }

            return entries.WeightedSelectRandom(entry => entry.weight).strategy;
        }
    }

    [System.Serializable]
    public struct PlayTableEntry
    {
        public CardPlayStrategy strategy;
        public float weight;
    }
}