using System;
using Sirenix.OdinInspector;

namespace Runtime.CardGameplay.Card
{
    [Serializable]
    public class CardInstance
    {
        public CardData Data;
        public int Rank;
        [ShowInInspector] public Suit Suit { get; set; }

        public CardController Controller { get; set; }

        public CardInstance(CardData data, int rank)
        {
            this.Data = data;
            this.Rank = rank;
            Suit = data.Suit;
            Controller = null;
        }
    }
}