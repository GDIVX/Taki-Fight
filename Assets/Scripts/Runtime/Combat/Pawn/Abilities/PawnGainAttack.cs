using System;
using UnityEngine;

namespace Runtime.Combat.Pawn.Abilities
{
    [CreateAssetMenu(fileName = "Gain Attack", menuName = "Pawns/Abilities/Buff/GainAttack", order = 0)]
    public class PawnGainAttack : PawnPlayStrategy
    {
        public override void Play(PawnController pawn, Action<bool> onComplete)
        {
            pawn.Combat.Damage.Value += Potency;
        }

        public override string GetDescription()
        {
            return $"+{Potency} Attack Power";
        }
    }
}