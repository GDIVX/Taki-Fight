﻿using System;
using UnityEngine;

namespace Runtime.Combat.Pawn.Abilities
{
    [CreateAssetMenu(fileName = "Pawn Win Combat", menuName = "Pawns/Abilities/GameState/WinCombat", order = 0)]
    public class PawnWinCombat : PawnPlayStrategy
    {
        public override void Play(PawnController pawn, Action<bool> onComplete)
        {
            GameManager.Instance.WinCombat();
            onComplete(true);
        }

        public override string GetDescription()
        {
            return "Win the combat.";
        }
    }
}