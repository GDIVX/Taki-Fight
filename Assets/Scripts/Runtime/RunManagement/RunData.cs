using System.Collections.Generic;
using Runtime.CardGameplay.Card;
using Runtime.CardGameplay.Deck;
using Runtime.Combat.Pawn;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Runtime.RunManagement
{
    [CreateAssetMenu(fileName = "Run Save File", menuName = "Game/RunData", order = 0)]
    public class RunData : ScriptableObject
    {
        [TableList] public List<CardData> Cards;
        public PawnData Hero;
        public bool IsRunInProgress { get; set; }
        public Deck Deck { get; set; }
    }
}