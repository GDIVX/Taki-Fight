﻿using System.Collections.Generic;
using Runtime.CardGameplay.Card.CardBehaviour;
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

        public CardPlayStrategy ChoseRandomPlayStrategy()
        {
            if (!entries.IsNullOrEmpty()) return entries.WeightedSelectRandom(entry => entry.weight).strategy;
            Debug.LogError($"{name} has no entries for the play table.");
            return default;

        }
    }
}