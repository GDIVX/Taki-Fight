using Runtime.CardGameplay.Card.CardBehaviour;
using UnityEngine;

namespace Runtime.Combat.Pawn.Enemy
{
    [System.Serializable]
    public struct PlayTableEntry
    {
        public CardPlayStrategy Strategy;
        public int Potency;
        public float Weight;
        public Sprite Sprite;
        public Color Color;
    }
}