using System;
using UnityEngine;

namespace Runtime.Combat.Pawn.Abilities
{
    [CreateAssetMenu(fileName = "Pawn Game Over", menuName = "Pawns/Abilities/GameState/GameOver", order = 0)]
    [Tooltip("End the game with defeat.")]
    public class PawnGameOver : PawnPlayStrategy
    {
        public override void Play(PawnController pawn, Action<bool> onComplete)
        {
            GameManager.Instance.EndRun();
            onComplete(true);
        }

        public override string GetDescription()
        {
            return "End the game.";
        }
    }
}
