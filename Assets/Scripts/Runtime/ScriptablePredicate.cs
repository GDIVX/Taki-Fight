using Runtime.CardGameplay.Card;
using Runtime.CardGameplay.Card.CardBehaviour.Predicates;
using UnityEngine;

namespace Runtime
{
    public abstract class ScriptablePredicate : ScriptableObject, IDescribable , IPredicate
    {
        public virtual bool Evaluate(PredicateHandler handler)
        {
            if (Negate)
            {
                return !OnEvaluate(handler);
            }

            return OnEvaluate(handler);
        }

        protected abstract bool OnEvaluate(PredicateHandler handler);
        public abstract string GetDescription();

        public bool Negate { get; set; }
    }

    public abstract class PredicateHandler
    {
    }

    public interface IPredicate
    {
        public bool Evaluate(PredicateHandler handler);
    }
}