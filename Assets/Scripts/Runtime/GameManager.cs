using System.Collections;
using Runtime.CardGameplay.Board;
using Runtime.CardGameplay.Deck;
using Runtime.Combat;
using Runtime.Combat.Pawn;
using Sirenix.OdinInspector;
using UnityEngine;
using Utilities;

namespace Runtime
{
    public class GameManager : Singleton<GameManager>
    {
        [SerializeField, Required, TabGroup("Dependencies")]
        private CardCollection cardCollection;

        [SerializeField, Required, TabGroup("Enemies")]
        private PawnFactory pawnFactory;

        [SerializeField, TabGroup("Hero"), Required]
        private PawnController heroPawn;

        public PawnController Hero => heroPawn;


        [Button]
        public void StartSession(PawnData data)
        {
            heroPawn.Init(data);
        }


        [Button]
        public void StartCombat(CombatConfig combatConfig)
        {
            SetupCardGameplay();
            
            //Spawn Enemies
            foreach (PawnController enemy in combatConfig.Enemies)
            {
                
            }
        }

        [Button]
        private void SetupCardGameplay()
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