using System;

namespace Runtime.CardGameplay.Card
{
    [Serializable]
    public struct CardInstance
    {
        public CardData Data { get; private set; }
        public int Number { get; set; }
        public Suit Suit { get; set; }

        public CardInstance(CardData data, int number)
        {
            Data = data;
            Number = number;
            Suit = data.Suit;
        }
    }
}