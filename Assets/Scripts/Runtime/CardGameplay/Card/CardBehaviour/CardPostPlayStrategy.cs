using UnityEngine;

namespace Runtime.CardGameplay.Card.CardBehaviour
{
    public abstract class CardPostPlayStrategy : ScriptableObject
    {
        public abstract void PostPlay(
            CardController cardController);
    }
}