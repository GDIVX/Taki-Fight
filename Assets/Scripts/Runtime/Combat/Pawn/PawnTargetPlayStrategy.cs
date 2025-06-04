using System;

namespace Runtime.Combat.Pawn
{
    public abstract class PawnTargetPlayStrategy : PawnPlayStrategy
    {
        public abstract void Play(PawnController pawn, PawnController target, Action<bool> onComplete);
    }
}
