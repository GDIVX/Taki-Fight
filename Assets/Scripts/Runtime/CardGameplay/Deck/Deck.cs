using System;
using System.Collections.Generic;
using Runtime.CardGameplay.Card;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Runtime.CardGameplay.Deck
{
    [Serializable]
    public class Deck
    {
        [ShowInInspector, ReadOnly] public Stack<CardInstance> DrawPile { get; private set; }
        [ShowInInspector, ReadOnly] public Stack<CardInstance> DiscardPile { get; private set; }

        public Deck(List<CardInstance> cards)
        {
            DrawPile = new Stack<CardInstance>(cards);
            DiscardPile = new Stack<CardInstance>();
        }
    }
}