using System;
using System.Threading.Tasks;
using UnityEngine;

namespace Runtime.CardGameplay.Card.CardBehaviour
{
    [CreateAssetMenu(fileName = "SimpleCardSelect", menuName = "Card/Strategy/Select/SimpleCardSelect")]
    public class SimpleCardSelect : CardSelectStrategy
    {
        public override void Select(CardController card, Action<bool> onSelectionComplete)
        {
            onSelectionComplete?.Invoke(true);
        }
    }
}