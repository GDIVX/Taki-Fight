using System;
using System.Collections;
using System.Collections.Generic;
using Runtime.CardGameplay.Card;
using Runtime.CardGameplay.Deck;
using Runtime.CardGameplay.GemSystem;
using Runtime.Combat;
using Runtime.Combat.Pawn;
using Runtime.Combat.Pawn.Enemy;
using Runtime.Events;
using Runtime.UI;
using Runtime.UI.Tooltip;
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
        private GemsBag _gemsBag;

        [SerializeField, Required, TabGroup("Dependencies")]
        private HandController _handController;


        [SerializeField, TabGroup("Settings")] private int _initialReelsCount;

        [SerializeField, Required, TabGroup("Dependencies")]
        private CardFactory _cardFactory;

        [SerializeField, Required] private CombatLane _enemiesLane;

        [SerializeField, TabGroup("Dependencies"), Required]
        private PawnController _heroPawn;

        [SerializeField, TabGroup("Dependencies"), Required]
        private BannerViewManager _bannerViewManager;

        [SerializeField, TabGroup("Tempt")] private GameObject _newGameButtonObject;

        private EventBus _eventBus;
        [SerializeField] private TooltipPool _tooltipPool;
        [SerializeField] private KeywordDictionary _keywordDictionary;


        public BannerViewManager BannerViewManager => _bannerViewManager;
        public GemsBag GemsBag => _gemsBag;
        public EventBus EventBus => _eventBus;
        public HandController Hand => _handController;

        public CombatLane Enemies => _enemiesLane;

        public PawnController Hero
        {
            get => _heroPawn;
            private set => _heroPawn = value;
        }


        public TooltipPool TooltipPool => _tooltipPool;

        public KeywordDictionary KeywordDictionary => _keywordDictionary;

        private void Awake()
        {
            _eventBus = new EventBus();
        }


        [Button]
        public void StartSession(PawnData data)
        {
            _cardCollection.Init();
            _heroPawn.gameObject.SetActive(true);
            _heroPawn.Init(data);
            _heroPawn.Health.OnDead += (sender, args) =>
            {
                _bannerViewManager.WriteMessage(0, "Defeat", Color.red);
                GameOver();
            };
            _enemiesLane.OnPawnRemoved += () =>
            {
                if (!_heroPawn.isActiveAndEnabled) return;
                if (_enemiesLane.Pawns.Count > 0) return;

                _bannerViewManager.WriteMessage(0, "Victory", Color.green);
                GameOver();
            };
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
        }

        private void GameOver()
        {
            _handController.gameObject.SetActive(false);
            _heroPawn.gameObject.SetActive(false);
            _enemiesLane.Clear();
            _newGameButtonObject.SetActive(true);
        }

        [Button]
        private void SetupCardGameplay()
        {
            _handController.gameObject.SetActive(true);
            _handController.DiscardHand();
            _cardCollection.CreateDeck();
            _gemsBag.Initialize(_cardCollection.Gems);
            StartTurn();
        }

        private void StartTurn()
        {
            BannerViewManager.WriteMessage(1, "Player Turn", Color.white);
            _gemsBag.OnTurnStart(() =>
            {
                BannerViewManager.Clear();
                _heroPawn.OnTurnStart();
                SetupEnemies();

                _handController.DrawHand();
            });
        }

        [Button]
        public void EndTurn()
        {
            //_handController.DiscardHand();
            _heroPawn.OnTurnEnd();
            _gemsBag.OnTurnEnd();

            PlayEnemiesTurn(() =>
            {
                //Reset the player defense 
                Hero.Defense.Value = 0;
                BannerViewManager.Clear();
                StartTurn();
            });
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

        private void PlayEnemiesTurn(Action onComplete)
        {
            if (Hero.Health.IsDead()) return;

            BannerViewManager.WriteMessage(1, "Enemies Turn", Color.yellow);
    
            var enemies = new Queue<PawnController>(_enemiesLane.Pawns);
            ProcessNextEnemy(enemies, onComplete);
        }

        private void ProcessNextEnemy(Queue<PawnController> enemies, Action onComplete)
        {
            if (enemies.Count == 0)
            {
                onComplete?.Invoke();
                return;
            }

            var enemy = enemies.Dequeue();
            enemy.Defense.Value = 0;
    
            if (enemy is EnemyController enemyController)
            {
                enemyController.PlayTurn(() => ProcessNextEnemy(enemies, onComplete));
            }
            else
            {
                Debug.LogError($"Failed to cast enemy {enemy} into interface {nameof(EnemyController)}");
                ProcessNextEnemy(enemies, onComplete);
            }
        }
    }
}