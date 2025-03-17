using System;
using Runtime.CardGameplay.Deck;
using Runtime.CardGameplay.GemSystem;
using Runtime.Combat.Pawn;

namespace Runtime.CardGameplay.Card
{
    public class CardDependencies
    {
        public HandController HandController { get; }
        public CardFactory CardFactory { get; }
        public Energy Energy { get; }


        public CardDependencies(HandController handController, Energy energy,
            CardFactory cardFactory)
        {
            HandController = handController ?? throw new ArgumentNullException(nameof(handController));
            Energy = energy ?? throw new ArgumentNullException(nameof(energy));
            CardFactory = cardFactory ?? throw new ArgumentNullException(nameof(cardFactory));
        }
    }
}