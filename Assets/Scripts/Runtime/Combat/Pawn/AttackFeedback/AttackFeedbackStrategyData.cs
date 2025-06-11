using System;
using UnityEngine;

namespace Runtime.Combat.Pawn.AttackFeedback
{
    [Serializable]
    public struct AttackFeedbackStrategyData
    {
        public AttackFeedbackStrategy Strategy;

        [SerializeReference]
        public StrategyParams Parameters;
    }
}
