using System;
using UnityEngine;

namespace Runtime.Combat.Pawn.Abilities
{
    [CreateAssetMenu(fileName = "Capture Ability", menuName = "Pawns/Abilities/Combat/Capture", order = 0)]
    public class CapturePawnAbility : PawnTargetPlayStrategy
    {
        public override void Play(PawnController pawn, PawnController target, Action<bool> onComplete)
        {
            if (target == null)
            {
                onComplete?.Invoke(false);
                return;
            }

            var success = target.Capture(Potency);
            onComplete?.Invoke(success);
        }

        public override void Play(PawnController pawn, Action<bool> onComplete)
        {
        }

        public override string GetDescription()
        {
            return "Capture a target pawn.";
        }
    }
}
