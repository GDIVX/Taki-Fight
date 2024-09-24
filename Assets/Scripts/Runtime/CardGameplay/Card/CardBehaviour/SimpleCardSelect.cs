using UnityEngine;

namespace Runtime.CardGameplay.Card.CardBehaviour
{
    [CreateAssetMenu(fileName = "Simple Select", menuName = "Card/Strategy/Select/Simple", order = 0)]
    public class SimpleCardSelect : CardSelectStrategy
    {
        public override bool Select(CardController card)
        {
            return true;
        }
    }
}