using System.Collections.Generic;
using Runtime.CardGameplay.Card;
using Runtime.CardGameplay.Deck;
using Runtime.Combat;
using Runtime.Combat.Pawn;

namespace Runtime.RunManagement
{
    public class GameRunState
    {
        public List<CardData> Cards;

        //TODO tempt
        public List<CombatConfig> Combats;
        public Deck Deck { get; set; }
        public School PrimarySchool { get; set; }
        public PawnData Mage { get; set; }
        public School SecondarySchool { get; set; }
    }
}