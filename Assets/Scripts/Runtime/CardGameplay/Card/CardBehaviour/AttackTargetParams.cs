using System;
using Runtime;

namespace Runtime.CardGameplay.Card.CardBehaviour
{
    [Serializable]
    public class AttackTargetParams : StrategyParams
    {
        public int TargetsCount;
        public TileFilterCriteria TileFilter;
    }
}
