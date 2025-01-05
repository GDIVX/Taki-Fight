﻿using System;
using Runtime.Combat.Pawn;
using Utilities;

namespace Runtime.Combat.StatusEffects
{
    public interface IStatusEffect
    {
        public TrackedProperty<int> Stack { get; set; }
        public void OnTurnStart(PawnController pawn);
        public void OnAdded(PawnController pawn);
        public void Remove(PawnController pawn);
    }
}