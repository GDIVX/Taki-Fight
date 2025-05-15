using System;
using Runtime.CardGameplay.Card;
using Runtime.CardGameplay.Deck;
using Runtime.CardGameplay.Energy;
using Runtime.Combat;
using Runtime.Rewards;
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
        [SerializeField] private RunData _runData;
        [SerializeField] private PlayerClassData _tempClassData;

        private static CardFactory CardFactory => ServiceLocator.Get<CardFactory>();
        private static CombatManager CombatManager => ServiceLocator.Get<CombatManager>();
        private static HandController HandController => ServiceLocator.Get<HandController>();
        private static RewardsOfferController RewardsOfferController => ServiceLocator.Get<RewardsOfferController>();

        public RunBuilder RunBuilder { get; private set; }
        public EventBus EventBus { get; private set; }
        public static GameManager Instance => ServiceLocator.Get<GameManager>();

        protected void Awake()
        {
            // Initialize core systems
            EventBus = new EventBus();
            ServiceLocator.Register(EventBus);
            RunBuilder = new RunBuilder(_runData);
            ServiceLocator.Register(RunBuilder);
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
            RunBuilder.NewRunFromPlayerClass(_tempClassData);

            var operation = SceneManager.LoadSceneAsync("Combat", LoadSceneMode.Additive);

            if (operation == null) throw new Exception("Failed to load combat scene!");

            operation.completed += _ =>
            {
                // Initialize run-specific services
                CardFactory.Init();
                RewardsOfferController.Init();
                CombatManager.Init();
            };
        }

        public void OnCombatStart()
        {
            var deckView = ServiceLocator.Get<DeckView>();
            var energy = ServiceLocator.Get<Energy>();

            var deck = _runData.Deck;
            HandController.Deck = deck;
            deckView.Setup(deck);
            HandController.Deck.MergeAndShuffle();

            energy.Initialize();
            energy.Reset();

            HandController.gameObject.SetActive(true);
            HandController.DrawHand();
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