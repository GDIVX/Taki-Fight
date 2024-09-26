using UnityEngine;

namespace Runtime.CardGameplay.Card.CardBehaviour
{
    public abstract class CardPlayStrategy : ScriptableObject
    {
        [SerializeField] private float duration = 1f;
        public abstract void Play(CardController card);
        public float Duration => duration;
    }
}