using System;
using UnityEngine;

namespace Runtime.Combat.Pawn.AttackFeedback
{
    [Serializable]
    public class AttackFeedbackParams : StrategyParams
    {
        public AnimationClip Animation;
        public GameObject VfxPrefab;
    }
}
