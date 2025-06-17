using System;
using System.Collections.Generic;

namespace Runtime.CardGameplay.Card
{
    [Serializable]
    public class CardInstance : IEqualityComparer<CardInstance>, ICloneable
    {
        public CardData Data;

        public CardInstanceState State;

        public CardInstance(CardData data)
        {
            Data = data;
            Controller = null;
            Cost = data.Cost;
            Guid = Guid.NewGuid();

            //default state
            State = CardInstanceState.Limbo;
        }

        public CardController Controller { get; set; }
        public int Cost { get; set; }
        public Guid Guid { get; private set; }


        public bool Equals(CardInstance x, CardInstance y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (x is null) return false;
            if (y is null) return false;
            return x.GetType() == y.GetType() && x.Guid.Equals(y.Guid);
        }

        public int GetHashCode(CardInstance obj)
        {
            return obj.Guid.GetHashCode();
        }

        public enum CardInstanceState
        {
            Limbo,
            Hand,
            Consumed,
            Discard,
            Draw
        }

        public object Clone()
        {
            var clone = new CardInstance(Data);
            clone.Cost = Cost;
            return clone;
        }
    }
}