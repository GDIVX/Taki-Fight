using System;
using UnityEngine;

namespace Runtime.Combat.Pawn
{
    public abstract class PawnPlayStrategy : ScriptableObject
    {
        public abstract void Play(PawnController pawn, int potency, Action<bool> OnComplete);
    }
}