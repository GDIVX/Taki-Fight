using System;
using UnityEngine;
using Runtime;

namespace Runtime.CardGameplay.Card.CardBehaviour
{
    public abstract class CardPlayStrategy : ScriptableObject, IDescribable
    {
        public int Potency { get; set; }
        protected StrategyParams Params { get; private set; }



        public virtual string GetDescription()
        {
            return "";
        }

        /// <summary>
        /// Executes the card effect. If the effect requires player input (e.g., selecting a target), it should call onComplete once finished.
        /// </summary>
        public abstract void Play(CardController cardController, Action<bool> onComplete);

        public virtual void Initialize(PlayStrategyData playStrategyData, CardController cardController)
        {
            Potency = playStrategyData.Potency;
            Params = playStrategyData.Parameters;


        }

        public virtual void BlindPlay(CardController cardController, Action<bool> onComplete)
        {
            Play(cardController, onComplete);
        }
    }
}