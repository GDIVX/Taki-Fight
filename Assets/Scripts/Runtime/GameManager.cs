using System.Collections;
using Runtime.CardGameplay.Board;
using Runtime.CardGameplay.Card;
using Runtime.CardGameplay.Deck;
using Runtime.Combat;
using Runtime.Combat.Pawn;
using Runtime.Combat.Pawn.Enemy;
using Sirenix.OdinInspector;
using UnityEngine;
using Utilities;

namespace Runtime
{
    public class GameManager : Singleton<GameManager>, IGameManager
    {
        [SerializeField, Required, TabGroup("Dependencies")]
        private CardCollection cardCollection;

        [SerializeField, Required, TabGroup("Dependencies")]
        private HandController _handController;

        [SerializeField, Required, TabGroup("Dependencies")]
        private BoardController _boardController;

        [SerializeField, Required] private CombatLane _enemiesLane;

        [SerializeField, TabGroup("Hero"), Required]
        private PawnController _heroPawn;


        public PawnController Hero
        {
            get => _heroPawn;
            private set => _heroPawn = value;
        }


        [Button]
        public void StartSession(PawnData data)
        {
            _heroPawn.Init(data);
        }


        [Button]
        public void StartCombat(CombatConfig combatConfig)
        {
            _enemiesLane.SpawnPawnsForCombat(combatConfig);
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
            _handController.DrawHand();
        }

        [Button]
        public void EndTurn()
        {
            StartCoroutine(ProcessEndTurn());
        }

        private IEnumerator ProcessEndTurn()
        {
            _boardController.OnTurnEnd();

            yield return PlayEnemiesTurn();

            //Reset the player defense 
            Hero.defense.Value = 0;
        }

        private IEnumerator PlayEnemiesTurn()
        {
            foreach (var enemy in (EnemyController[])_enemiesLane.Pawns)
            {
                yield return enemy.ChoseAndPlayStrategy();
                enemy.defense.Value = 0;
            }
        }
    }
}