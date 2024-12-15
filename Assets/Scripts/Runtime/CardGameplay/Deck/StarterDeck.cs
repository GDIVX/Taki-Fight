using System.Collections.Generic;
using System.Linq;
using Runtime.CardGameplay.Card;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Runtime.CardGameplay.Deck
{
    [CreateAssetMenu(fileName = "Starter Deck", menuName = "Card/Starter Deck", order = 0)]
    public class StarterDeck : ScriptableObject
    {
        [SerializeField, TableList] private List<CardData> _cards;

        public List<CardData> Cards => _cards;


        [Button]
        public void Clear()
        {
            _cards.Clear();
        }
    }
}