using System;
using Runtime.Combat.Pawn;
using Runtime;
namespace Runtime.CardGameplay.Card.CardBehaviour
{
    [Serializable]
    public class SummonUnitParams : StrategyParams
    {
        public PawnData Unit;
        public TileFilterCriteria TileFilter;
    }
}
