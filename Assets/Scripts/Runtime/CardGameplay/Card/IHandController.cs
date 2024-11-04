namespace Runtime.CardGameplay.Card
{
    public interface IHandController
    {
        bool Has(CardController card);
        void DiscardCard(CardController card);
    }
}