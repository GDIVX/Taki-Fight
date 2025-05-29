using System;
using Runtime.CardGameplay.Card;
using Runtime.CardGameplay.Deck;
using Runtime.CardGameplay.Energy;
using Runtime.Combat;
using Runtime.RunManagement;
using Runtime.SceneManagementExtend;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utilities;
using EventBus = Runtime.Events.EventBus;

namespace Runtime
{
    public class GameManager : MonoService<GameManager>
    {
        [SerializeField] private School _tempClassData;
        [SerializeField] private GameRunState _gameRunState;

        private static CardFactory CardFactory => ServiceLocator.Get<CardFactory>();
        private static CombatManager CombatManager => ServiceLocator.Get<CombatManager>();
        private static HandController HandController => ServiceLocator.Get<HandController>();

        public EventBus EventBus { get; private set; }
        public static GameManager Instance => ServiceLocator.Get<GameManager>();

        protected void Awake()
        {
            // Initialize core systems
            EventBus = new EventBus();
            ServiceLocator.Register(EventBus);
            OnEventBusCreated?.Invoke();
        }

        private void Start()
        {
            //Load the main menu
            SceneManager.LoadScene("MainMenu", LoadSceneMode.Additive);
        }

        public event Action OnEventBusCreated;

        public void StartRun()
        {
            _gameRunState = new RunBuilder().WithPrimarySchool(_tempClassData).Build();

            var operation = SceneManager.LoadSceneAsync("Combat", LoadSceneMode.Additive);

            if (operation == null) throw new Exception("Failed to load combat scene!");

            operation.completed += _ =>
            {
                // Initialize run-specific services
                CardFactory.Init();
                CombatManager.Init();
            };
        }

        public void OnCombatStart()
        {
            var deckView = ServiceLocator.Get<DeckView>();
            var energy = ServiceLocator.Get<Energy>();

            var deck = _gameRunState.Deck;
            HandController.Deck = deck;
            deckView.Setup(deck);
            HandController.Deck.MergeAndShuffle();

            energy.Initialize();
            energy.Reset();

            HandController.gameObject.SetActive(true);
        }

        private void OnCombatEnd()
        {
            CombatManager.EndCombat();
            HandController.DiscardHand();
        }

        public void WinCombat()
        {
            OnCombatEnd();
            //TODO: progression
            Debug.Log("Win");
        }

        [Button]
        public void EndRun()
        {
            //reboot the game by loading the bootstrap scene single
            SceneManagerExtensions.SafeLoadSceneAsync("Bootstrap", LoadSceneMode.Single);
        }
    }
}