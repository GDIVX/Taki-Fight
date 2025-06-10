using System;
using Runtime.CardGameplay.Card;
using UnityEngine;
using Runtime;

namespace Runtime.Combat.Pawn
{
    public abstract class PawnPlayStrategy : ScriptableObject, IDescribable
    {
        protected int Potency { get; private set; }
        public StrategyParams Params { get; private set; }

        public virtual string GetDescription()
        {
            return "";
        }

        public abstract void Play(PawnController pawn, Action<bool> onComplete);

        public virtual void Initialize(PawnStrategyData data)
        {
            Potency = data.Potency;
            Params = data.Parameters;
        }
    }
}
