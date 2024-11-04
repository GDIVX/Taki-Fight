using Runtime.CardGameplay.Card.CardBehaviour;

namespace Runtime.Combat.Pawn.Enemy
{
    [System.Serializable]
    public struct PlayTableEntry
    {
        public CardPlayStrategy strategy;
        public float weight;
    }
}