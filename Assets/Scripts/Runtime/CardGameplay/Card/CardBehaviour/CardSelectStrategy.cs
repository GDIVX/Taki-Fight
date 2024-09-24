using UnityEngine;

namespace Runtime.CardGameplay.Card.CardBehaviour
{
    public abstract class CardSelectStrategy : ScriptableObject
    {
        public abstract bool Select(CardController card);
    }
}