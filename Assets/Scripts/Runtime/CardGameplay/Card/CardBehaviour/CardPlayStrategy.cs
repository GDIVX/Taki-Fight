using Runtime.Combat.Pawn;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Runtime.CardGameplay.Card.CardBehaviour
{
    public abstract class CardPlayStrategy : ScriptableObject
    {
        public abstract void Play(PawnController caller, int potency);
    }
}