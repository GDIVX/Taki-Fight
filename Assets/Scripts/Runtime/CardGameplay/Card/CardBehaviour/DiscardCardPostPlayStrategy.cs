using UnityEngine;

namespace Runtime.CardGameplay.Card.CardBehaviour
{
    [CreateAssetMenu(fileName = "Default Post Play", menuName = "Card/Strategy/PostPlay/Default")]
    public class DiscardCardPostPlayStrategy : CardPostPlayStrategy
    {
        public override void PostPlay(
            CardController cardController)
        {
            cardController.HandController.DiscardCard(cardController);
        }
    }
}