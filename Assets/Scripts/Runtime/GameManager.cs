using System.Collections;
using System.Collections.Generic;
using Runtime.CardGameplay.Board;
using Runtime.CardGameplay.Deck;
using Runtime.Combat;
using Runtime.Combat.Pawn;
using Runtime.Combat.Pawn.Enemy;
using Sirenix.OdinInspector;
using UnityEngine;
using Utilities;

namespace Runtime
{
    public class GameManager : Singleton<GameManager>
    {
        [SerializeField, Required, TabGroup("Dependencies")]
        private CardCollection cardCollection;

        [SerializeField, Required] private CombatLane enemiesLane;

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
            enemiesLane.SpawnPawnsForCombat(combatConfig);
            SetupCardGameplay();
        }

        [Button]
        public void EndCombat()
        {
            //TODO
            Debug.Log("End Combat");
        }

        [Button]
        private void SetupCardGameplay()
        {
            cardCollection.CreateDeck();
            HandController.Instance.DrawHand();
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

            foreach (var enemy in enemiesLane.Pawns)
            {
                var enemyAI = enemy.GetComponent<EnemyController>();
                yield return enemyAI.ChoseAndPlayStrategy();
                enemy.defense.Value = 0;
            }

            //Reset the player defense 
            Hero.defense.Value = 0;
        }
    }
}