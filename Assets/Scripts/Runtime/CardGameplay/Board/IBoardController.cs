using Runtime.CardGameplay.Card;

namespace Runtime.CardGameplay.Board
{
    public interface IBoardController
    {
        bool CanPlayCard(ICardController card);
        void UpdateCurrentSuitAndRank(ICardController card);
    }
}