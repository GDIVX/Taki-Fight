using Runtime.CardGameplay.Card.CardBehaviour;
using Runtime.CardGameplay.Card.CardBehaviour.Feedback;
using UnityEngine;

namespace Runtime.Combat.Pawn.Enemy
{
    [System.Serializable]
    public struct PlayTableEntry
    {
        public CardPlayStrategy Strategy;
        public FeedbackStrategy FeedbackStrategy;
        public int Potency;
        public IntentionType IntentionType;
        public bool AddAttackMod, AddDefenseMod;
        public bool AddHealingMod;
        public float Weight;
        public int Repeats;
    }
}