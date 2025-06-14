using System;
using Runtime.Combat.Pawn;

namespace Runtime.CardGameplay.Card.CardBehaviour
{
    [Serializable]
    public class GetPawnsParams : StrategyParams
    {
        public int TargetsCount;
        public PawnOwner PawnOwner;
    }
}