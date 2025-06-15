using System;
using Runtime.CardGameplay.Deck;
using UnityEngine;
using Utilities;

namespace Runtime.CardGameplay.Card.CardBehaviour
{
    [CreateAssetMenu(fileName = "Draw And Play", menuName = "Card/Strategy/Play/Draw And Play", order = 0)]
    public class DrawAndPlayStrategy : CardPlayStrategy
    {
        public override void Play(CardController cardController, Action<bool> onComplete)
        {
            var handController = ServiceLocator.Get<HandController>();
            if (handController == null)
            {
                Debug.LogError("HandController not found.");
                onComplete?.Invoke(false);
                return;
            }

            for (int i = 0; i < Potency; i++)
            {
                var drawn = handController.DrawCard();
                drawn?.Play();
            }

            onComplete?.Invoke(true);
        }

        public override string GetDescription()
        {
            return Potency > 1 ? $"Draw and play {Potency} cards" : "Draw and play a card";
        }
    }
}
