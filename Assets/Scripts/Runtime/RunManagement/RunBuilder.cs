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

        public void NewRunFromPlayerClass(PlayerClassData playerClassData)
        {
            AddHeroData(playerClassData.Pawn);
            Data.Cards = playerClassData.StarterCards;
            AddDeck();
            Data.IsRunInProgress = true;
        }

        private RunBuilder AddHeroData(PawnData pawnData)
        {
            Data.Hero = pawnData;
            return this;
        }

        private void AddDeck()
        {
            var deck = CreateDeck();
            Data.Deck = deck;
        }

        private Deck CreateDeck()
        {
            var cards = Data.Cards.Select(data => new CardInstance(data)).ToList();
            var deck = new Deck(cards);
            return deck;
        }
    }
}