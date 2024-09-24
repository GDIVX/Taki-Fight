using System.Collections.Generic;
using Runtime.CardGameplay.Card;
using UnityEngine;

namespace Runtime.CardGameplay.Deck
{
    [CreateAssetMenu(fileName = "Starter Deck", menuName = "Card/Starter Deck", order = 0)]
    public class StarterDeck : ScriptableObject
    {
        [SerializeField] private List<CardInstance> cards;

        public List<CardInstance> Cards => cards;
    }
}