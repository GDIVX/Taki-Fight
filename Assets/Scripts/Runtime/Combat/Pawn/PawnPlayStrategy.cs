using System;
using Runtime.CardGameplay.Card;
using UnityEngine;

namespace Runtime.Combat.Pawn
{
    public abstract class PawnPlayStrategy : ScriptableObject, IDescribable
    {
        [SerializeField] private string _description;

        public virtual string GetDescription()
        {
            return "";
        }

        public abstract void Play(PawnController pawn, int potency, Action<bool> onComplete);
    }
}