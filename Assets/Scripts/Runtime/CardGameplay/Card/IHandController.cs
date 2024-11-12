namespace Runtime.CardGameplay.Card
{
    public interface IHandController
    {
        bool Has(ICardController card);
        void DiscardCard(ICardController card);
    }
}