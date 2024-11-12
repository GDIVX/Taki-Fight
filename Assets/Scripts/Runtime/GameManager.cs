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
        private CardCollection _cardCollection;

        [SerializeField, Required, TabGroup("Dependencies")]
        private HandController _handController;

        [SerializeField, Required, TabGroup("Dependencies")]
        private BoardController _boardController;

        [SerializeField, Required, TabGroup("Dependencies")]
        private CardFactory _cardFactory;

        [SerializeField, Required] private CombatLane _enemiesLane;

        [SerializeField, TabGroup("Dependencies"), Required]
        private PawnController _heroPawn;


        public PawnController Hero
        {
            get => _heroPawn;
            private set => _heroPawn = value;
        }

        public Suit CurrentSuit => _boardController.CurrentSuit;


        [Button]
        public void StartSession(PawnData data)
        {
            _heroPawn.Init(data);
            _cardFactory.Init(_heroPawn);
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
            _cardCollection.CreateDeck();
            _handController.DrawHand();
        }

        [Button]
        public void EndTurn()
        {
            StartCoroutine(ProcessEndTurn());
        }

        private IEnumerator ProcessEndTurn()
        {
            Debug.Log("Started process turn");
            _boardController.OnTurnEnd();

            yield return StartCoroutine(PlayEnemiesTurn());

            //Reset the player defense 
            Hero.Defense.Value = 0;
        }

        private IEnumerator PlayEnemiesTurn()
        {
            Debug.Log("Started Play Enemy Turn");
            foreach (var enemy in _enemiesLane.Pawns)
            {
                if (enemy is IAiBrain brain)
                {
                    Debug.Log("Casted enemy to brain");
                    yield return StartCoroutine(brain.ChoseAndPlayStrategy());
                }
                else
                {
                    Debug.LogError($"Failed to cast enemy {enemy} into interface {nameof(IAiBrain)}");
                }

                enemy.Defense.Value = 0;
            }
        }
    }
}