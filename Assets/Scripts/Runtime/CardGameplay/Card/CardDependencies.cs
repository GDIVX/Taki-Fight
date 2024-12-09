using System;
using Runtime.CardGameplay.Board;
using Runtime.Combat.Pawn;

namespace Runtime.CardGameplay.Card
{
    public class CardDependencies
    {
        public IHandController HandController { get; }
        public IBoardController BoardController { get; }
        public PawnController Pawn { get; }
        public ICardFactory CardFactory { get; }
        

        public CardDependencies(IHandController handController, IBoardController boardController, PawnController pawn, ICardFactory cardFactory)
        {
            HandController = handController ?? throw new ArgumentNullException(nameof(handController));
            BoardController = boardController ?? throw new ArgumentNullException(nameof(boardController));
            Pawn = pawn ?? throw new ArgumentNullException(nameof(pawn));
            CardFactory = cardFactory ?? throw new ArgumentNullException(nameof(cardFactory));
        }
    }
}