using System;
using System.Collections.Generic;
using System.Linq;
using Runtime.CardGameplay.Card;
using Runtime.CardGameplay.Deck;
using Runtime.Combat.Pawn;

namespace Runtime.RunManagement
{
    public class RunBuilder
    {
        private RunData Data { get; }

        public RunBuilder(RunData data)
        {
            Data = data ?? throw new ArgumentNullException(nameof(data));
        }

        public void Reset()
        {
            Data.Hero = null;
            Data.Cards.Clear();
            Data.IsRunInProgress = false;
        }

        public void NewRunFromPlayerClass(PlayerClassData playerClassData)
        {
            if (playerClassData == null) throw new ArgumentNullException(nameof(playerClassData));

            AddHeroData(playerClassData.Pawn);
            Data.Cards = new List<CardData>(playerClassData.StarterCards);
            Data.CollectableCards = new List<CardData>(playerClassData.CollectableCards);
            SetDeck();
            Data.IsRunInProgress = true;
        }

        public void AddCard(CardData cardData)
        {
            if (cardData == null) throw new ArgumentNullException(nameof(cardData));
            Data.Cards.Add(cardData);
            SetDeck();
        }

        public void RemoveCard(CardData cardData)
        {
            if (cardData == null) throw new ArgumentNullException(nameof(cardData));
            Data.Cards.Remove(cardData);
            SetDeck();
        }

        private void AddHeroData(PawnData pawnData)
        {
            if (pawnData == null) throw new ArgumentNullException(nameof(pawnData));
            Data.Hero = pawnData;
        }

        private void SetDeck()
        {
            Data.Deck = new Deck(Data.Cards.Select(data => new CardInstance(data)).ToList());
        }
    }
}