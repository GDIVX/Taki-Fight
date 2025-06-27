using System;
using System.Collections.Generic;
using UnityEngine;
using Runtime;
using Runtime.Combat.Pawn;
using Runtime.Combat.Tilemap;

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

        /// <summary>
        /// Called when a card is initialized. Override to initialize custom variables.
        /// </summary>
        /// <param name="playStrategyData"></param>
        /// <param name="cardController"></param>
        public virtual void Initialize(PlayStrategyData playStrategyData, CardController cardController)
        {
            Potency = playStrategyData.Potency;
            Params = playStrategyData.Parameters;
        }

        /// <summary>
        /// Used for when this card is played blindly but other systems or cards.
        /// For example, if another card has an effect to play a random card in the hand.
        /// </summary>
        /// <param name="cardController"></param>
        /// <param name="onComplete"></param>
        public virtual void BlindPlay(CardController cardController, Action<bool> onComplete)
        {
            Play(cardController, onComplete);
        }


        /// <summary>
        /// Helper class to define tiles that this card can be played on.
        /// Used by view to highlight tiles.
        /// </summary>
        /// <returns></returns>
        public virtual bool IsValidTile(Tile tile)
        {
            return false;
        }

        /// <summary>
        /// Helper class to define cards that this card can be played on.
        /// Used by view to highlight cards.
        /// </summary>
        /// <param name="otherCard"></param>
        /// <returns></returns>
        public virtual bool IsAskingForCard(CardController otherCard)
        {
            return false;
        }
    }
}