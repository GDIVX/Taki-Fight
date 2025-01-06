using System;
using System.Collections.Generic;
using System.Linq;
using Runtime.CardGameplay.ManaSystem;
using Sirenix.OdinInspector;

namespace Runtime.CardGameplay.Card
{
    [Serializable]
    public class CardInstance
    {
        public CardData Data;
        [ShowInInspector] public List<Mana> Cost { get; set; }

        public CardController Controller { get; set; }


        public CardInstance(CardData data)
        {
            Data = data;
            Controller = null;
            Cost = data.GetCost();
        }
    }
}