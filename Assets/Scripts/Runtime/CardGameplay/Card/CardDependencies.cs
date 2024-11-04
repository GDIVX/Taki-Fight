using System;

namespace Runtime.CardGameplay.Card
{
    public class CardDependencies
    {
        public IHandController HandController { get; }
        public IBoardController BoardController { get; }
        public IGameManager GameManager { get; }
        public ICardFactory CardFactory { get; }

        public CardDependencies(IHandController handController, IBoardController boardController, IGameManager gameManager, ICardFactory cardFactory)
        {
            HandController = handController ?? throw new ArgumentNullException(nameof(handController));
            BoardController = boardController ?? throw new ArgumentNullException(nameof(boardController));
            GameManager = gameManager ?? throw new ArgumentNullException(nameof(gameManager));
            CardFactory = cardFactory ?? throw new ArgumentNullException(nameof(cardFactory));
        }
    }
}