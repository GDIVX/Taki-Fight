using System;
using UnityEngine;

namespace Runtime.CardGameplay.Card.CardBehaviour
{
    [CreateAssetMenu(fileName = "On Retained", menuName = "Card/Strategy/Triggers/Retained", order = 0)]
    public class OnRetainedPlayStrategy : CardPlayStrategy
    {
        private SecondaryActionParams _params;

        public override void Play(CardController cardController, Action<bool> onComplete)
        {
            //DO nothing
            cardController.OnRetained -= OnCardRetained;
            onComplete(true);
        }

        public override void Initialize(PlayStrategyData playStrategyData, CardController cardController)
        {
            base.Initialize(playStrategyData, cardController);

            if (playStrategyData.Parameters is SecondaryActionParams secondaryActionParams)
            {
                _params = secondaryActionParams;
            }

            cardController.OnRetained += OnCardRetained;
        }

        private void OnCardRetained(CardController cardController)
        {
            cardController.OnRetained -= OnCardRetained;

            if (_params.PlayStrategyData.PlayStrategy)
            {
                _params.PlayStrategyData.PlayStrategy.Play(cardController, b =>
                {
                    if (_params.ConsumeCard)
                    {
                        cardController.Consume();
                    }
                    else
                    {
                        cardController.Discard();
                    }
                });
                return;
            }

            if (_params.ConsumeCard)
            {
                cardController.Consume();
            }
        }

        public override string GetDescription()
        {
            var builder = new DescriptionBuilder();
            builder.WithKeyword("Retained");

            if (_params.PlayStrategyData.PlayStrategy)
            {
                builder.WithLine(_params.PlayStrategyData.PlayStrategy);
            }

            if (_params.ConsumeCard)
            {
                builder.WithKeyword("Consume");
            }

            return builder.GetFormattedText();
        }
    }

    [Serializable]
    public class SecondaryActionParams : StrategyParams
    {
        public PlayStrategyData PlayStrategyData;
        public bool ConsumeCard;
    }
}