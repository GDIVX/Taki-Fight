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
        [SerializeField, TableList] private List<CardInstance> cards;


        public List<CardInstance> Clone()
        {
            return cards.Select(card => new CardInstance(card.Data, card.Rank)).ToList();
        }

        [Button]
        public void AddCardSeries(CardData data, int numberFrom, int numberTo, int copies)
        {
            for (int i = 0; i < copies; i++)
            {
                for (int number = numberFrom; number <= numberTo; number++)
                {
                    CardInstance card = new CardInstance(data, number);
                    cards.Add(card);
                }
            }
        }

        [Button]
        public void Clear()
        {
            cards.Clear();
        }
    }
}