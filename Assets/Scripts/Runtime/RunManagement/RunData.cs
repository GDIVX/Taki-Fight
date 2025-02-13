using System.Collections.Generic;
using Runtime.CardGameplay.Card;
using Runtime.CardGameplay.Deck;
using Runtime.Combat;
using Runtime.Combat.Pawn;
using Runtime.Rewards;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Runtime.RunManagement
{
    [CreateAssetMenu(fileName = "Run Save File", menuName = "Game/RunData", order = 0)]
    public class RunData : ScriptableObject
    {
        [TabGroup("Settings"), TableList] public List<RarityToWeightEntry> RarityToWightEntries;
        [TabGroup("Settings")] public int CardsToRewardCount;
        [TableList] public List<CardData> Cards;
        [ReadOnly] public List<CardData> CollectableCards;
        public PawnData Hero;
        public bool IsRunInProgress { get; set; }
        public Deck Deck { get; set; }

        //TODO tempt
        public List<CombatConfig> Combats;
    }
}