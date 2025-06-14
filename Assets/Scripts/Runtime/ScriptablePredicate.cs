using UnityEngine;

namespace Runtime
{
    public abstract class ScriptablePredicate<T> : ScriptableObject where T : PredicateHandler
    {
        public abstract bool Evaluate(T predicateParams);
    }

    public abstract class PredicateHandler
    {
    }
}