using UnityEngine;

namespace Runtime.CardGameplay.Card.CardBehaviour
{
    public abstract class CardAffordabilityStrategy : ScriptableObject
    {
        public abstract bool CanPlayCard(CardController cardController);
    }
}