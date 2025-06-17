using System;
using System.Collections.Generic;
using Runtime.CardGameplay.Deck;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;
using Utilities;

namespace Runtime.CardGameplay.Card.CardBehaviour
{
    [CreateAssetMenu(fileName = "Create Card", menuName = "Card/Strategy/Play/Create Card", order = 0)]
    public class CreateCardPlayStrategy : CardPlayStrategy
    {
        private CardCreationParams _params;

        public override void Play(CardController cardController, Action<bool> onComplete)
        {
            bool success = false;
            for (int i = 0; i < Potency; i++)
            {
                CreateNewCard(cardController, out success);
            }

            onComplete(success);
        }

        private void CreateNewCard(CardController cardController, out bool success)
        {
            CardController newCard;

            //create a new card instance 
            if (_params.Clone)
            {
                newCard = cardController.Clone() as CardController;
            }
            else
            {
                var cardFactory = ServiceLocator.Get<CardFactory>();
                newCard = cardFactory.Create(_params.CardData);
            }

            if (newCard == null)
            {
                Debug.LogWarning("Card could not be created");
                success = false;
                return;
            }

            //Modify strategies if needed
            var strategies = new List<PlayStrategyData>(newCard.PlayStrategies);
            strategies.AddRange(_params.InjectedStrategies);
            var tempList = new List<PlayStrategyData>();
            for (var i = 0; i < strategies.Count && i < _params.ModifyPotencyBy.Count; i++)
            {
                var strategy = strategies[i];

                if (strategy.PlayStrategy is CreateCardPlayStrategy) continue;

                strategy.Potency += _params.ModifyPotencyBy[i];
                tempList.Add(strategy);
            }

            strategies.Clear();
            strategies.AddRange(tempList);

            newCard.PlayStrategies = strategies;
            newCard.Cost += _params.ModifyCostBy;

            if (_params.MakeConsume)
            {
                newCard.IsConsumed = true;
            }

            var hand = ServiceLocator.Get<HandController>();
            hand.AddCardToHand(newCard);
            success = true;
        }

        public override void Initialize(PlayStrategyData playStrategyData, CardController cardController)
        {
            base.Initialize(playStrategyData, cardController);
            _params = playStrategyData.Parameters as CardCreationParams;
        }

        public override string GetDescription()
        {
            //TODO: Temp. Create a tooltip that display a card.
            return _params.Clone ? "Clone Card" : "Create a new Card";
        }

        [Serializable]
        public class CardCreationParams : StrategyParams
        {
            public bool Clone;
            [ShowIf("IsOriginal")] public CardData CardData;
            public List<int> ModifyPotencyBy;
            public int ModifyCostBy;
            public bool MakeConsume;
            public List<PlayStrategyData> InjectedStrategies;

            private bool IsOriginal => !Clone;
        }
    }
}