using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;

namespace Runtime.CardGameplay.Card
{
    [Serializable]
    public class CardInstance
    {
        public CardData Data;

        public CardController Controller { get; set; }
        public int Cost { get; set; }


        public CardInstance(CardData data)
        {
            Data = data;
            Controller = null;
            Cost = data.Cost;
        }
    }
}