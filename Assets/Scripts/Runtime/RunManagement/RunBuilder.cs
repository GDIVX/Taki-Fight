using System.Linq;
using Runtime.CardGameplay.Card;
using Runtime.CardGameplay.Deck;
using Runtime.Combat.Pawn;

namespace Runtime.RunManagement
{
    public class RunBuilder
    {
        public RunData Data { get; private set; }

        public RunBuilder(RunData data)
        {
            Data = data;
        }

        public void Reset()
        {
            Data.Hero = null;
            Data.Cards.Clear();
            Data.IsRunInProgress = false;
        } 

        public RunBuilder AddHeroData(PawnData pawnData)
        {
            Data.Hero = pawnData;
            return this;
        }

        public void SetupNewDeckDependencies(HandController controller, DeckView deckView)
        {
            var deck = CreateDeck();
            controller.Deck = deck;
            deckView.Setup(deck);
        }

        private Deck CreateDeck()
        {
            var cards = Data.Cards.Select(data => new CardInstance(data)).ToList();
            var deck = new Deck(cards);
            return deck;
        }
    }
}