using System;
using UnityEngine;

namespace Runtime.CardGameplay.Card.CardBehaviour
{
    public abstract class CardPlayStrategy : ScriptableObject, IDescribable
    {
        public virtual string GetDescription()
        {
            return "";
        }

        /// <summary>
        /// Executes the card effect. If the effect requires player input (e.g., selecting a target), it should call onComplete once finished.
        /// </summary>
        public abstract void Play(CardController cardController, int potency, Action<bool> onComplete);
    }
}