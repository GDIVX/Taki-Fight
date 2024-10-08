using System.Collections;
using Runtime.CardGameplay.Board;
using Runtime.CardGameplay.Deck;
using Runtime.Combat.Pawn;
using Sirenix.OdinInspector;
using UnityEngine;
using Utilities;

namespace Runtime.CardGameplay
{
    public class CombatManager : Singleton<CombatManager>
    {
        [SerializeField, Required, TabGroup("Dependencies")]
        private CardCollection cardCollection;

        [SerializeField, TabGroup("Hero"), Required]
        private PawnController heroPawn;

        public PawnController Hero => heroPawn;


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
            StartCoroutine(ProcessEndTurn());
        }

        private IEnumerator ProcessEndTurn()
        {
            //Play the cards
            BoardController.Instance.OnTurnEnd();

            yield return new WaitUntil(() => !BoardController.Instance.IsProcessingTurn());

            //TODO: play the enemy turn

            //Reset the player defense 
            Hero.defense.Value = 0;
        }
    }
}