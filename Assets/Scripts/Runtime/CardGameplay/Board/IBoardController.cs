namespace Runtime.CardGameplay.Card
{
    public interface IBoardController
    {
        bool CanPlayCard(CardController card);
        void UpdateMatch(CardController card);
    }
}