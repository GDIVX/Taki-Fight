using System;
using Runtime.CardGameplay.Deck;
using Runtime.CardGameplay.ManaSystem;
using Runtime.Combat.Pawn;

namespace Runtime.CardGameplay.Card
{
    public class CardDependencies
    {
        public HandController HandController { get; }
        public PawnController Pawn { get; }
        public CardFactory CardFactory { get; }
        public ManaInventory ManaInventory { get; }


        public CardDependencies(HandController handController, ManaInventory manaInventory, PawnController pawn,
            CardFactory cardFactory)
        {
            HandController = handController ?? throw new ArgumentNullException(nameof(handController));
            ManaInventory = manaInventory ?? throw new ArgumentNullException(nameof(manaInventory));
            Pawn = pawn ?? throw new ArgumentNullException(nameof(pawn));
            CardFactory = cardFactory ?? throw new ArgumentNullException(nameof(cardFactory));
        }
    }
}