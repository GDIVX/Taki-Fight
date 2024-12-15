using UnityEngine;

namespace Runtime.CardGameplay.Card.CardBehaviour
{
    public abstract class CardOnRankChangedEventHandler : ScriptableObject
    {
        public abstract void OnRankChanged(int rank);
    }
}