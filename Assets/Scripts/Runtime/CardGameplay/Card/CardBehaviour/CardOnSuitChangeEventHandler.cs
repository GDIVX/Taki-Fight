using UnityEngine;

namespace Runtime.CardGameplay.Card.CardBehaviour
{
    public abstract class CardOnSuitChangeEventHandler : ScriptableObject
    {
        public abstract void OnSuitChanged(CardGlyph cardGlyph);
    }
}