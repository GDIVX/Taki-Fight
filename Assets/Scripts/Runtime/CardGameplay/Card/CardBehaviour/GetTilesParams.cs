using System;
using Runtime;

namespace Runtime.CardGameplay.Card.CardBehaviour
{
    [Serializable]
    public class GetTilesParams : StrategyParams
    {
        public int TargetsCount;
        public TileFilterCriteria TileFilter;
    }
}
