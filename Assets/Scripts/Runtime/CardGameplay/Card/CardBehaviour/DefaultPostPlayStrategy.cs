using UnityEngine;

namespace Runtime.CardGameplay.Card.CardBehaviour
{
    [CreateAssetMenu(fileName = "Default Post Play", menuName = "Card/Strategy/PostPlay/Default")]
    public class DefaultPostPlayStrategy : CardPostPlayStrategy
    {
        public override void PostPlay(
            CardController cardController)
        {
            cardController.GlyphBoardController.UpdateGlyphs(cardController.Glyphs);
            cardController.HandController.DiscardCard(cardController);
        }
    }
}