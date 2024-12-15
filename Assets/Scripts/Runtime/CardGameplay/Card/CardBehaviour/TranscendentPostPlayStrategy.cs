using UnityEngine;

namespace Runtime.CardGameplay.Card.CardBehaviour
{
    [CreateAssetMenu(fileName = "Transcendent Post Play Strategy", menuName = "Card/Strategy/PostPlay/Transcendent",
        order = 0)]
    public class TranscendentPostPlayStrategy : CardPostPlayStrategy
    {
        public override void PostPlay(CardController cardController)
        {
            cardController.HandController.DiscardCard(cardController);
        }
    }
}