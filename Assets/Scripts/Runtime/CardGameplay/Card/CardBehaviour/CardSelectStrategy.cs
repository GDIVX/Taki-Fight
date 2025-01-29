using System;
using System.Threading.Tasks;
using Runtime.Combat.Pawn.Targeting;
using UnityEngine;

namespace Runtime.CardGameplay.Card.CardBehaviour
{
    public abstract class CardSelectStrategy : ScriptableObject
    {
        public abstract void Select(CardController card, Action<bool> onSelectionComplete);
    }
}