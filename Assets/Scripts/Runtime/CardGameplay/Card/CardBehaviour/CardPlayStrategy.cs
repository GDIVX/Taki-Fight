using System;
using UnityEngine;
using Runtime;

namespace Runtime.CardGameplay.Card.CardBehaviour
{
    public abstract class CardPlayStrategy : ScriptableObject, IDescribable
    {
        public int Potency { get; set; }
        public StrategyParams Params { get; private set; }

        public virtual string GetDescription()
        {
            return "";
        }

        /// <summary>
        ///     Executes the card effect. If the effect requires player input (e.g., selecting a target), it should call
        ///     onComplete once finished.
        /// </summary>
        public abstract void Play(CardController cardController, Action<CardPlayResult> onComplete);

        public virtual void Initialize(PlayStrategyData playStrategyData)
        {
            Potency = playStrategyData.Potency;
            Params = playStrategyData.Parameters;
        }
    }
}