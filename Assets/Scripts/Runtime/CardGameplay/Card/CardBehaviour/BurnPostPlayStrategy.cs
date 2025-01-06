using UnityEngine;

namespace Runtime.CardGameplay.Card.CardBehaviour
{
    [CreateAssetMenu(fileName = "Burn Post Play Strategy", menuName = "Card/Strategy/PostPlay/Burn", order = 0)]
    public class BurnPostPlayStrategy : CardPostPlayStrategy
    {
        public override void PostPlay(CardController cardController)
        {
            cardController.HandController.BurnCard(cardController);
        }
    }
}