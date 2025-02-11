using System;
using Runtime.CardGameplay.Card;
using Runtime.CardGameplay.Deck;
using Runtime.CardGameplay.GemSystem;
using Runtime.Combat;
using Runtime.Combat.Pawn;
using Runtime.Events;
using Runtime.RunManagement;
using Runtime.UI;
using Runtime.UI.Tooltip;
using Sirenix.OdinInspector;
using UnityEngine;
using Utilities;
using EventBus = Runtime.Events.EventBus;

namespace Runtime
{
    public class GameManager : Singleton<GameManager>, IGameManager
    {
        [SerializeField, Required, TabGroup("Dependencies")]
        private GemsBag _gemsBag;

        [SerializeField, Required, TabGroup("Dependencies")]
        private HandController _handController;

        [SerializeField, Required, TabGroup("Dependencies")]
        private DeckView _deckView;


        [SerializeField, TabGroup("Settings")] private int _initialReelsCount;

        [SerializeField, Required, TabGroup("Dependencies")]
        private CardFactory _cardFactory;

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
        public GemsBag GemsBag => _gemsBag;

        public HandController Hand => _handController;

        public PawnController Hero
        {
            set => _combatManager.Hero = value;
            get => _combatManager.Hero;
        }

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
            CombatManager.InitializeHero(_runData.Hero);
            _cardFactory.Init(CombatManager.Hero);
        }


        private void GameOver()
        {
            _handController.gameObject.SetActive(false);
            _newGameButtonObject.SetActive(true);
        }

        [Button]
        public void SetupCardGameplay()
        {
            var deck = _runData.Deck;
            _handController.Deck = deck;
            _deckView.Setup(deck);
            _gemsBag.Initialize();
            _handController.gameObject.SetActive(true);
            _handController.DiscardHand();
            _gemsBag.Clear();
        }
    }
}