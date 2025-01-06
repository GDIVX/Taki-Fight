using UnityEngine;

namespace Runtime.CardGameplay.Card.CardBehaviour
{
    [CreateAssetMenu(fileName = "Default Affordability Strategy", menuName = "Card/Strategy/Affordability/Default")]
    public class DefaultAffordabilityStrategy : CardAffordabilityStrategy
    {
        public override bool CanPlayCard(CardController cardController)
        {
            return cardController.ManaInventory.Contains(cardController.Cost);
        }
    }
}