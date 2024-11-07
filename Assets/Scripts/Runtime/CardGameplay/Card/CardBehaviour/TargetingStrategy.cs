using Runtime.Combat.Pawn;
using Runtime.Combat.Pawn.Targeting;
using UnityEngine;

namespace Runtime.CardGameplay.Card.CardBehaviour
{
    public abstract class TargetingStrategy : ScriptableObject
    {
        public abstract PawnController GetTarget();
    }
}