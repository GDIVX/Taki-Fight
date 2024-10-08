using System.Threading.Tasks;
using UnityEngine;

namespace Runtime.CardGameplay.Card.CardBehaviour
{
    [CreateAssetMenu(fileName = "SimpleCardSelect", menuName = "Card/Strategy/Select/SimpleCardSelect")]
    public class SimpleCardSelect : CardSelectStrategy
    {
        public override Task<bool> SelectAsync(CardController card)
        {
            // Simple case where no additional selection is required
            return Task.FromResult(true);
        }
    }
}