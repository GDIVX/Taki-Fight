using System;
using System.Collections.Generic;
using System.Linq;
using Runtime.CardGameplay.Card;
using Utilities;

namespace Runtime.Rewards
{
    public class CardRewardManager
    {
        private readonly List<CardData> _availableCards;
        private readonly Dictionary<Rarity, float> _weights;


        public CardRewardManager(List<CardData> allCards, Dictionary<Rarity, float> weights)
        {
            if (allCards == null || allCards.Count == 0)
                throw new ArgumentException("Card pool cannot be null or empty.", nameof(allCards));

            if (weights == null || weights.Count == 0)
                throw new ArgumentException("Weights cannot be null or empty.", nameof(weights));

            _availableCards = allCards.Distinct().ToList(); // Prevent duplicates upfront
            _weights = new Dictionary<Rarity, float>(weights);
        }

        public List<CardData> OfferCardReward(int count)
        {
            if (count <= 0)
                throw new ArgumentOutOfRangeException(nameof(count), "Card reward count must be greater than zero.");

            List<CardData> chosenCards = SelectCards(count);
            return chosenCards;
        }

        private List<CardData> SelectCards(int count)
        {
            List<CardData> selectedCards = new List<CardData>();
            HashSet<CardData> usedCards = new HashSet<CardData>();

            int availableCount = Math.Min(count, _availableCards.Count);
            
            for (int i = 0; i < availableCount; i++)
            {
                CardData card;
                do
                {
                    card = _availableCards.WeightedSelectRandom(data => _weights.GetValueOrDefault(data.Rarity, 1f));
                }
                while (usedCards.Contains(card) && usedCards.Count < _availableCards.Count);

                usedCards.Add(card);
                selectedCards.Add(card);
            }

            return selectedCards;
        }
    }
}
