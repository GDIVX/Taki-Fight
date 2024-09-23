namespace Runtime.CardGameplay.Card.CardBehaviour
{
    public class SimpleOnCardSelect : IOnCardSelectStrategy
    {
        public bool OnSelect()
        {
            return true;
        }
    }
}