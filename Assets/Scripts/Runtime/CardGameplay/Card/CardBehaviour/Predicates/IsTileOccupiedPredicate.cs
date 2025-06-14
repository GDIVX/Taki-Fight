using System;
using Runtime.Combat.Tilemap;
using UnityEngine;

namespace Runtime.CardGameplay.Card.CardBehaviour.Predicates
{
    [CreateAssetMenu(fileName = "Is Tile Occupied", menuName = "Game/Predicates/Is Tile Occupied", order = 0)]
    public class IsTileOccupiedPredicate : ScriptablePredicate<IsTileOccupiedPredicateHandler>
    {
        public override bool Evaluate(IsTileOccupiedPredicateHandler predicateParams)
        {
            return predicateParams != null && predicateParams.Tile.IsOccupied;
        }
    }

    [Serializable]
    public class IsTileOccupiedPredicateHandler : PredicateHandler
    {
        public Tile Tile;
    }
}