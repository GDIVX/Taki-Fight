using System;
using UnityEngine;

namespace Runtime.Combat.Pawn.AttackFeedback
{
    [Serializable]
    public class MeleeAttackFeedbackParams : AttackFeedbackParams
    {
        public float MoveDuration = 0.25f;
    }
}
