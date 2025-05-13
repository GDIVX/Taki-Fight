using System;
using System.Collections.Generic;
using System.Linq;
using Runtime.CardGameplay.Card;
using Runtime.RunManagement;
using UnityEngine;
using Utilities;

namespace Runtime.Rewards
{
    public class RewardsOfferController : MonoService<RewardsOfferController>
    {
        [SerializeField] private RunData _runData;
        [SerializeField] private RewardsView _rewardsView;
        private CardRewardManager _cardRewardManager;
        private Action _onRewardComplete;
        private RunBuilder _runBuilder;

        public void Init()
        {
            if (_runData == null)
            {
                Debug.LogError("RunData is missing in RewardsOfferController!");
                return;
            }

            _cardRewardManager = new CardRewardManager(_runData.CollectableCards, CreateRarityWeightDictionary());

            if (!ServiceLocator.TryGet(out RunBuilder runBuilder))
            {
                Debug.LogError("Failed to find RunBuilder in ServiceLocator.");
                return;
            }

            _runBuilder = runBuilder;
        }

        public void OfferRewards(Action onComplete)
        {
            if (_cardRewardManager == null)
            {
                Debug.LogError("CardRewardManager is not initialized.");
                onComplete?.Invoke();
                return;
            }

            var cards = _cardRewardManager.OfferCardReward(_runData.CardsToRewardCount);

            if (cards.Count == 0)
            {
                Debug.LogWarning("No available cards to offer.");
                onComplete?.Invoke();
                return;
            }

            _rewardsView.ShowCardRewardSelection(cards, ChoseReward, SkipReward);

            _onRewardComplete = onComplete;
        }

        private void ChoseReward(CardData cardData)
        {
            if (cardData == null)
            {
                Debug.LogWarning("Chosen card is null.");
                return;
            }

            if (_runBuilder == null)
            {
                Debug.LogError("RunBuilder is missing! Cannot add card.");
                return;
            }

            _runBuilder.AddCard(cardData);
            _onRewardComplete?.Invoke();
        }

        private void SkipReward()
        {
            Debug.Log("Player skipped the reward.");
            _onRewardComplete?.Invoke();
        }

        private Dictionary<Rarity, float> CreateRarityWeightDictionary()
        {
            var rarityWeights = new Dictionary<Rarity, float>();

            foreach (var entry in _runData.RarityToWightEntries.Where(entry =>
                         !rarityWeights.TryAdd(entry.Rarity, entry.Weight)))
            {
                Debug.LogWarning($"Duplicate rarity detected: {entry.Rarity}. Using the first assigned weight.");
            }

            return rarityWeights;
        }
    }

    [Serializable]
    public struct RarityToWeightEntry
    {
        public Rarity Rarity;
        public float Weight;
    }
}