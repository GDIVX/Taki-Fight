namespace Runtime.CardGameplay.Card
{
    public struct CardInstance
    {
        public CardData Data { get; private set; }
        public int Number { get; private set; }

        public CardInstance(CardData data, int number)
        {
            Data = data;
            Number = number;
        }
    }
}