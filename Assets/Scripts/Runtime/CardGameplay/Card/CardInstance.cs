using System;

namespace Runtime.CardGameplay.Card
{
    [Serializable]
    public struct CardInstance
    {
        public CardData data;
        public int number;
        public Suit Suit { get; set; }

        public CardInstance(CardData data, int number)
        {
            this.data = data;
            this.number = number;
            Suit = data.Suit;
        }
    }
}