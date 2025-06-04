using System;

namespace Runtime.Combat.Pawn
{
    public abstract class PawnHitPlayStrategy : PawnPlayStrategy
    {
        public abstract void Play(PawnController pawn, PawnController target, ref int damage, Action<bool> onComplete);
    }
}
