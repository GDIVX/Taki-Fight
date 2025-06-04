using System;
using Runtime.CardGameplay.Card;
using UnityEngine;

namespace Runtime.Combat.Pawn
{
    public abstract class PawnPlayStrategy : ScriptableObject, IDescribable
    {
        public int Potency { get; protected set; }

        public virtual string GetDescription()
        {
            return "";
        }

        public abstract void Play(PawnController pawn, Action<bool> onComplete);

        public virtual void Initialize(PawnStrategyData data)
        {
            Potency = data.Potency;
        }
    }
}