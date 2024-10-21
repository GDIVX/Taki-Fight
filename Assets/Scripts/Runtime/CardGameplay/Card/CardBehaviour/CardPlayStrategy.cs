using Runtime.Combat.Pawn;
using UnityEngine;

namespace Runtime.CardGameplay.Card.CardBehaviour
{
    public abstract class CardPlayStrategy : ScriptableObject
    {
        [SerializeField] private float duration = 1f;
        public float Duration => duration;
        public abstract void Play(PawnController caller);
    }
}