using Runtime.CardGameplay.Board;
using Runtime.CardGameplay.Deck;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Runtime.CardGameplay
{
    public class CombatManager : MonoBehaviour
    {
        [SerializeField, TabGroup("Dependencies")]
        private CardCollection cardCollection;

        [Button]
        public void StartCombat()
        {
            cardCollection.CreateDeck();
            HandController.Instance.DrawHand();
        }

        [Button]
        public void EndTurn()
        {
            DiscardBoardIfSequenceIsNotIntact();
        }

        private static void DiscardBoardIfSequenceIsNotIntact()
        {
            if (!BoardController.Instance.IsSequenceIsIntact())
            {
                BoardController.Instance.DiscardAll();
            }
        }
    }
}