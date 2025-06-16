using System;
using Runtime.CardGameplay.Card.View;
using Runtime.Combat.Pawn;
using Utilities;

namespace Runtime.Combat.StatusEffects
{
    public interface IStatusEffect : ICloneable
    {
        public Observable<int> Stack { get; set; }
        Keyword Keyword { get; set; }
        public void OnTurnStart(PawnController pawn);
        public void OnAdded(PawnController pawn);
        public void Remove(PawnController pawn);
    }
}