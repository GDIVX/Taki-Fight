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
            BoardController.Instance.OnTurnStart();
        }

        [Button]
        public void EndTurn()
        {
            BoardController.Instance.OnTurnEnd();
        }
    }
}