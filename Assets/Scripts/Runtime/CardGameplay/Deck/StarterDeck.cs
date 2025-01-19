using System.Collections.Generic;
using Runtime.CardGameplay.Card;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Runtime.CardGameplay.Deck
{
    [CreateAssetMenu(fileName = "Starter Deck", menuName = "Card/Starter Deck", order = 0)]
    public class StarterDeck : ScriptableObject
    {
        [SerializeField, TableList] private List<CardData> _cards;
        [SerializeField, BoxGroup("Gems")] private GemGroup _gems;


        public List<CardData> Cards => _cards;
        public GemGroup Gems => _gems;


        [Button]
        public void Clear()
        {
            _cards.Clear();
        }
    }
}