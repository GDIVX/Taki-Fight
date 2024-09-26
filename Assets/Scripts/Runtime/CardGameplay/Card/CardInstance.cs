using System;
using Sirenix.OdinInspector;

namespace Runtime.CardGameplay.Card
{
    [Serializable]
    public class CardInstance
    {
        public CardData data;
        public int number;
        [ShowInInspector] public Suit Suit { get; set; }

        public CardController Controller { get; set; }

        public CardInstance(CardData data, int number)
        {
            this.data = data;
            this.number = number;
            Suit = data.Suit;
            Controller = null;
        }
    }
}