using Runtime.CardGameplay.Board;
using Runtime.CardGameplay.Deck;
using Runtime.Combat.Pawn;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Runtime.CardGameplay
{
    public class CombatManager : MonoBehaviour
    {
        [SerializeField, Required, TabGroup("Dependencies")]
        private CardCollection cardCollection;

        [SerializeField, TabGroup("Hero"), Required]
        private PawnController heroPawn;


        [Button]
        public void StartSession(PawnData data)
        {
            heroPawn.Init(data);
        }


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