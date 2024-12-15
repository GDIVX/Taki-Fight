using System;
using Runtime.CardGameplay.Deck;
using Runtime.CardGameplay.GlyphsBoard;
using Runtime.Combat.Pawn;

namespace Runtime.CardGameplay.Card
{
    public class CardDependencies
    {
        public HandController HandController { get; }
        public GlyphBoardController GlyphBoardController { get; }
        public PawnController Pawn { get; }
        public CardFactory CardFactory { get; }
        

        public CardDependencies(HandController handController, GlyphBoardController glyphBoardController, PawnController pawn, CardFactory cardFactory)
        {
            HandController = handController ?? throw new ArgumentNullException(nameof(handController));
            GlyphBoardController = glyphBoardController ?? throw new ArgumentNullException(nameof(glyphBoardController));
            Pawn = pawn ?? throw new ArgumentNullException(nameof(pawn));
            CardFactory = cardFactory ?? throw new ArgumentNullException(nameof(cardFactory));
        }
    }
}