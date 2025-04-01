using System;
using Runtime.CardGameplay.Card;
using Runtime.CardGameplay.Deck;
using Runtime.CardGameplay.Energy;
using Runtime.Combat;
using Runtime.Combat.Pawn;
using Runtime.Events;
using Runtime.Rewards;
using Runtime.RunManagement;
using Runtime.UI;
using Runtime.UI.Tooltip;
using Sirenix.OdinInspector;
using UnityEngine;
using Utilities;
using EventBus = Runtime.Events.EventBus;

namespace Runtime
{
    public class GameManager : Singleton<GameManager>
    {
        [SerializeField, Required, TabGroup("Dependencies")]
        private Energy _energy;

        [SerializeField, Required, TabGroup("Dependencies")]
        private HandController _handController;

        [SerializeField, Required, TabGroup("Dependencies")]
        private DeckView _deckView;


        [SerializeField, TabGroup("Settings")] private int _initialReelsCount;

        [SerializeField, Required, TabGroup("Dependencies")]
        private CardFactory _cardFactory;

        [SerializeField, Required, TabGroup("Dependencies")]
        private RewardsOfferController _rewardsOfferController;

        [SerializeField, TabGroup("Dependencies"), Required]
        private BannerViewManager _bannerViewManager;

        [SerializeField, TabGroup("Dependencies"), Required]
        private CombatManager _combatManager;


        [SerializeField, TabGroup("Tempt")] private GameObject _newGameButtonObject;

        [SerializeField] private TooltipPool _tooltipPool;
        [SerializeField] private KeywordDictionary _keywordDictionary;
        [SerializeField] private RunData _runData;

        public RunBuilder RunBuilder { get; private set; }
        public BannerViewManager BannerViewManager => _bannerViewManager;
        public Energy Energy => _energy;

        public HandController Hand => _handController;

        public EventBus EventBus { get; private set; }

        public TooltipPool TooltipPool => _tooltipPool;

        public KeywordDictionary KeywordDictionary => _keywordDictionary;

        public CombatManager CombatManager => _combatManager;
        public event Action OnEventBusCreated;

        //event listeners
        private EventListener<GameStateEvent> _onGameStateChange;


        //TODO: TEMP
        [SerializeField] private PlayerClassData _tempClassData;

        private void Awake()
        {
            EventBus = new EventBus();
            ServiceLocator.Register(EventBus);
            RunBuilder = new RunBuilder(_runData);
            ServiceLocator.Register(RunBuilder);
            OnEventBusCreated?.Invoke();

            ServiceLocator.Register(Energy);
            ServiceLocator.Register(_cardFactory);
            ServiceLocator.Register(_handController);
        }

        private void OnEnable()
        {
            _onGameStateChange = new EventListener<GameStateEvent>(e =>
            {
                var gameState = e.GameState;

                switch (gameState)
                {
                    case GameState.Menu:
                        break;
                    case GameState.RunStart:
                        OnStartRun();
                        break;
                    case GameState.Pause:
                        break;
                    case GameState.Exploration:
                        break;
                    case GameState.RoomEntered:
                        break;
                    case GameState.RoomResolved:
                        break;
                    case GameState.Combat:
                        break;
                    case GameState.CombatEnd:
                        break;
                    case GameState.RunEnd:
                        GameOver();
                        break;
                    case GameState.Hub:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            });
        }

        private void OfferCardReward()
        {
            _rewardsOfferController.OfferRewards(() =>
            {
                //TODO: replace with exploration and progression
                _combatManager.StartCombat(_runData.Combats.SelectRandom());
            });
        }

        /// <summary>
        /// We are going to use the currently saved run. For a new run, create a new file
        /// </summary>
        public void StartRun()
        {
            //TODO: TEMP
            RunBuilder.NewRunFromPlayerClass(_tempClassData);

            SetGameState(GameState.RunStart);
        }

        private void OnDisable()
        {
            _onGameStateChange.Disable();
        }


        [Button]
        public void SetGameState(GameState gameState)
        {
            EventBus.Publish(new GameStateEvent(gameState));
            Debug.Log($"Set state to {gameState}");
        }


        private void OnStartRun()
        {
            // CombatManager.InitializeHero(_runData.Hero);
            _cardFactory.Init();
            _rewardsOfferController.Init();
        }


        private void GameOver()
        {
            _handController.gameObject.SetActive(false);
            _newGameButtonObject.SetActive(true);
        }

        [Button]
        public void OnCombatStart()
        {
            var deck = _runData.Deck;
            _handController.Deck = deck;
            _deckView.Setup(deck);
            _handController.Deck.MergeAndShuffle();

            _energy.Initialize();
            _energy.Reset();

            _handController.gameObject.SetActive(true);
            _handController.DiscardHand();
        }

        public void OnCombatEnd()
        {
            SetGameState(GameState.CombatEnd);
            _handController.DiscardHand();
            OfferCardReward();
        }
    }
}