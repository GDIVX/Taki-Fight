using Runtime.CardGameplay.Card.CardBehaviour;

namespace Runtime.CardGameplay.Card
{
    /// <summary>
    /// Handle The behaviour of a card
    /// </summary>
    public class Card
    {
        public int Number { get; private set; }
        public Suit Suit { get; private set; }
        public bool Selectable { get; set; }

        private IOnCardSelectStrategy _onCardSelectStrategy;
        private IOnCardPlayStrategy _onCardPlayStrategy;

        public Card(int number, Suit suit)
        {
            Number = number;
            Suit = suit;
        }

        public void Select()
        {
            //If the card is not selectable, return
            if (!Selectable) return;

            //handle card selection
            if (_onCardSelectStrategy.OnSelect())
            {
                //if valid, add it to the sequence
                //Table.Instance.AddToSequence(this);
                //Hand.Instance.Remove(this);
            }
            else
            {
                //else, add it to the hand
                //Hand.Instance.Add(this);
                //Table.Instance.Remove(this);
            }
        }

        public void Play()
        {
            _onCardPlayStrategy.Execute();
        }
    }

    public enum Suit
    {
        Red,
        Blue,
        Purple,
        Yellow,
        Green
    }
}