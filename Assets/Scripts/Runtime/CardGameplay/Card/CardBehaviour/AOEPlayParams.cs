using System;
using Runtime.Combat.Tilemap;
using UnityEngine;

namespace Runtime.CardGameplay.Card.CardBehaviour
{
    [Serializable]
    public class AOEPlayParams : StrategyParams
    {
        public Vector2Int AreaSize;
        public TileFilterCriteria TileFilter;
    }
}
