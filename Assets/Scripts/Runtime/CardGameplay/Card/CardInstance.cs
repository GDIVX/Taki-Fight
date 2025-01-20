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
        [ShowInInspector] public GemGroup Group { get; set; }

        public CardController Controller { get; set; }


        public CardInstance(CardData data)
        {
            Data = data;
            Controller = null;
            Group = data.Group;
        }
    }
}