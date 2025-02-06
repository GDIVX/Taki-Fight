using System;
using Runtime.CardGameplay.Card;
using Runtime.CardGameplay.Deck;
using Runtime.CardGameplay.GemSystem;
using Runtime.Combat.Pawn;
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

        [SerializeField, TabGroup("Dependencies"), Required]
        private BannerViewManager _bannerViewManager;

        [SerializeField, TabGroup("Dependencies"), Required]
        private CombatManager _combatManager;


        [SerializeField, TabGroup("Tempt")] private GameObject _newGameButtonObject;

        private EventBus _eventBus;
        [SerializeField] private TooltipPool _tooltipPool;
        [SerializeField] private KeywordDictionary _keywordDictionary;
        [SerializeField] private RunData _runData;

        public BannerViewManager BannerViewManager => _bannerViewManager;
        public GemsBag GemsBag => _gemsBag;
        public EventBus EventBus => _eventBus;
        public HandController Hand => _handController;

        public PawnController Hero
        {
            set => _combatManager.Hero = value;
            get => _combatManager.Hero;
        }


        public TooltipPool TooltipPool => _tooltipPool;

        public KeywordDictionary KeywordDictionary => _keywordDictionary;

        public CombatManager CombatManager => _combatManager;
        public event Action OnEventBusCreated;

        //event listeners
        private EventListener<GameStateEvent> _onGameStateChange;

        private void Awake()
        {
            _eventBus = new EventBus();
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
                        StartSession();
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
            SetGameState(GameState.RunStart);
        }

        private void OnDisable()
        {
            _onGameStateChange.Disable();
        }


        [Button]
        public void SetGameState(GameState gameState)
        {
            _eventBus.Publish(new GameStateEvent(gameState));
            Debug.Log($"Set state to {gameState}");
        }


        private void StartSession()
        {
            CombatManager.InitializeHero(_runData.Hero);
            _cardCollection.Init();
            _gemsBag.Initialize();
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
            _handController.gameObject.SetActive(true);
            _handController.DiscardHand();
            _cardCollection.CreateDeck();
            _gemsBag.Clear();
        }
    }
}