using System;
using Runtime.Combat.Pawn;

namespace Runtime.CardGameplay.Card.CardBehaviour
{
    [Serializable]
    public class SummonUnitParams : StrategyParams
    {
        public PawnData Unit;
    }
}