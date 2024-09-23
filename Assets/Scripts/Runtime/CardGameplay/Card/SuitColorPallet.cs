using System;
using System.Collections.Generic;
using UnityEngine;

namespace Runtime.CardGameplay.Card
{
    [CreateAssetMenu(fileName = "SuitColorPallet", menuName = "SuitColorPallet", order = 0)]
    public class SuitColorPallet : ScriptableObject
    {
        [SerializeField] private List<SuitColor> pallet;

        public Color GetColor(Suit suit)
        {
            return pallet.Find(p => p.suit == suit).color;
        }
    }

    [Serializable]
    public struct SuitColor
    {
        public Suit suit;
        public Color color;
    }
}