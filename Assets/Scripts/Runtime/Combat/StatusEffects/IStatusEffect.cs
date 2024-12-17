using System;
using Runtime.Combat.Pawn;

namespace Runtime.Combat.StatusEffects
{
    public interface IStatusEffect
    {
        public event Action<IStatusEffect> OnApplied;
        public event Action<IStatusEffect> OnRemoved;
        public int Stack { get; set; }
        public void Apply(PawnController pawn);
        public void OnAdded(PawnController pawn);
        public void Remove(PawnController pawn);
    }
}