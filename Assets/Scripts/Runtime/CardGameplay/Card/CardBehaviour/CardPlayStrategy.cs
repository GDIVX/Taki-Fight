using Runtime.Combat.Pawn;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Runtime.CardGameplay.Card.CardBehaviour
{
    public abstract class CardPlayStrategy : ScriptableObject
    {
        [SerializeField] private float _duration = 1f;
        public float Duration => _duration;


        public abstract void Play(PawnController caller, int potency);

    }
}