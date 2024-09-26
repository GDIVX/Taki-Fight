using System.Collections.Generic;
using Runtime.CardGameplay.Card;
using UnityEngine;
using Utilities;

namespace Runtime.CardGameplay.Board
{
    [CreateAssetMenu(fileName = "Random Card Values", menuName = "Card/Random Values", order = 0)]
    public class RandomCardValuesPicker : ScriptableObject
    {
        [SerializeField] public List<Suit> suits;
        [SerializeField] private int smallestNumber, biggestNumber;

        public (Suit, int) GetRandomValues()
        {
            var suit = suits.PickRandom();
            var number = Random.Range(smallestNumber, biggestNumber);

            return (suit, number);
        }
    }
}