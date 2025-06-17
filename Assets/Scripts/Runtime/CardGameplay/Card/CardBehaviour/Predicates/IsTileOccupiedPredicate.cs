using System;
using Runtime.Combat.Tilemap;
using UnityEngine;

namespace Runtime.CardGameplay.Card.CardBehaviour.Predicates
{
    [CreateAssetMenu(fileName = "Is Tile Occupied", menuName = "Game/Predicates/Is Tile Occupied", order = 0)]
    public class IsTileOccupiedPredicate : ScriptablePredicate, ITileEvaluate
    {
        protected override bool OnEvaluate(PredicateHandler handler)
        {
            return handler is ITileGetter tileGetter && tileGetter.GetTile().IsOccupied;
        }

        public override string GetDescription()
        {
            return "If Tile Occupied: ";
        }

        public bool Evaluate(ITileGetter tileGetter)
        {
            return tileGetter.GetTile().IsOccupied;
        }

        public bool Evaluate(Tile tile)
        {
            return tile.IsOccupied;
        }
    }

    [Serializable]
    public class IsTileOccupiedPredicateHandler : PredicateHandler, ITileGetter
    {
        public Tile Tile;

        public Tile GetTile()
        {
            return Tile;
        }
    }

    public interface ITileGetter
    {
        public Tile GetTile();
    }

    public interface ITileEvaluate : IPredicate
    {
        public bool Evaluate(ITileGetter tileGetter);
        public bool Evaluate(Tile tile);
    }
}