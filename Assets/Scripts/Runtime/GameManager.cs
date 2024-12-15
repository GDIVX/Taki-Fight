using System;
using System.Collections;
using Runtime.CardGameplay.Card;
using Runtime.CardGameplay.Deck;
using Runtime.CardGameplay.GlyphsBoard;
using Runtime.Combat;
using Runtime.Combat.Pawn;
using Runtime.Combat.Pawn.Enemy;
using Runtime.Events;
using Runtime.UI;
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
        private GlyphBoardController _glyphBoardController;

        [SerializeField, Required, TabGroup("Dependencies")]
        private CardFactory _cardFactory;

        [SerializeField, Required] private CombatLane _enemiesLane;

        [SerializeField, TabGroup("Dependencies"), Required]
        private PawnController _heroPawn;

        [SerializeField, TabGroup("Dependencies"), Required]
        private BannerViewManager _bannerViewManager;

        private EventBus _eventBus;


        public BannerViewManager BannerViewManager => _bannerViewManager;
        public EventBus EventBus => _eventBus;

        public PawnController Hero
        {
            get => _heroPawn;
            private set => _heroPawn = value;
        }

        private void Awake()
        {
            _eventBus = new EventBus();
        }


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
            StartTurn();
        }

        private void StartTurn()
        {
            StartCoroutine(ProcessStartTurn());
        }

        [Button]
        public void EndTurn()
        {
            StartCoroutine(ProcessEndTurn());
        }

        private IEnumerator ProcessStartTurn()
        {
            BannerViewManager.WriteMessage(1, "Player Turn");
            yield return new WaitForSeconds(1);
            BannerViewManager.Clear();
            SetupEnemies();

            _handController.DrawHand();
        }

        private void SetupEnemies()
        {
            foreach (var enemy in _enemiesLane.Pawns)
            {
                if (enemy is EnemyController enemyController)
                {
                    enemyController.ChoosePlayStrategy();
                }
                else
                {
                    Debug.LogError($"Failed to cast enemy {enemy} into interface {nameof(EnemyController)}");
                }
            }
        }

        private IEnumerator ProcessEndTurn()
        {
            _handController.DiscardHand();

            yield return StartCoroutine(PlayEnemiesTurn());

            //Reset the player defense 
            Hero.Defense.Value = 0;
        }

        private IEnumerator PlayEnemiesTurn()
        {
            BannerViewManager.WriteMessage(1, "Enemies Turn");
            yield return new WaitForSeconds(1);
            BannerViewManager.Clear();
            foreach (var enemy in _enemiesLane.Pawns)
            {
                enemy.Defense.Value = 0;
                if (enemy is EnemyController enemyController)
                {
                    yield return StartCoroutine(enemyController.PlayTurn());
                }
                else
                {
                    Debug.LogError($"Failed to cast enemy {enemy} into interface {nameof(EnemyController)}");
                }
            }

            StartTurn();
        }
    }
}