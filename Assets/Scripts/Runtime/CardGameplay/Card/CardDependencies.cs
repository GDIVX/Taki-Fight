using System;
using Runtime.CardGameplay.Deck;
using Runtime.CardGameplay.GemSystem;
using Runtime.Combat.Pawn;

namespace Runtime.CardGameplay.Card
{
    public class CardDependencies
    {
        public HandController HandController { get; }
        public PawnController Pawn { get; }
        public CardFactory CardFactory { get; }
        public GemsBag GemsBag { get; }


        public CardDependencies(HandController handController, GemsBag gemsBag, PawnController pawn,
            CardFactory cardFactory)
        {
            HandController = handController ?? throw new ArgumentNullException(nameof(handController));
            GemsBag = gemsBag ?? throw new ArgumentNullException(nameof(gemsBag));
            Pawn = pawn ?? throw new ArgumentNullException(nameof(pawn));
            CardFactory = cardFactory ?? throw new ArgumentNullException(nameof(cardFactory));
        }
    }
}