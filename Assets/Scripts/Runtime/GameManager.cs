using System;
using Runtime.CardGameplay.Card;
using Runtime.CardGameplay.Deck;
using Runtime.CardGameplay.Energy;
using Runtime.Combat;
using Runtime.Events;
using Runtime.Rewards;
using Runtime.RunManagement;
using Sirenix.OdinInspector;
using UnityEngine;
using Utilities;
using EventBus = Runtime.Events.EventBus;

namespace Runtime
{
    public class GameManager : Singleton<GameManager>
    {

        [SerializeField, TabGroup("Tempt")] private GameObject _newGameButtonObject;
        [SerializeField] private RunData _runData;

        public RunBuilder RunBuilder { get; private set; }
        public EventBus EventBus { get; private set; }

        public event Action OnEventBusCreated;


        //TODO: TEMP
        [SerializeField] private PlayerClassData _tempClassData;

        private RewardsOfferController _rewardsOfferController;
        private CombatManager _combatManager;
        private CardFactory _cardFactory;
        private HandController _handController;

        private void Awake()
        {
            EventBus = new EventBus();
            ServiceLocator.Register(EventBus);
            RunBuilder = new RunBuilder(_runData);
            ServiceLocator.Register(RunBuilder);
            OnEventBusCreated?.Invoke();
        }

        private void Start()
        {
            _rewardsOfferController = ServiceLocator.Get<RewardsOfferController>();
            _combatManager = ServiceLocator.Get<CombatManager>();
            _cardFactory = ServiceLocator.Get<CardFactory>();
            _handController = ServiceLocator.Get<HandController>();
        }


        private void OfferCardReward()
        {
            _rewardsOfferController.OfferRewards(() =>
            {
                //TODO: replace with exploration and progression
                _combatManager.StartCombat();
            });
        }

        /// <summary>
        /// We are going to use the currently saved run. For a new run, create a new file
        /// </summary>
        public void StartRun()
        {
            //TODO: TEMP
            RunBuilder.NewRunFromPlayerClass(_tempClassData);

            _cardFactory.Init();
            _rewardsOfferController.Init();
            _combatManager.Init();
        }

        private void GameOver()
        {

            _handController.gameObject.SetActive(false);
            _newGameButtonObject.SetActive(true);
        }

        [Button]
        public void OnCombatStart()
        {

            var deckView = ServiceLocator.Get<DeckView>();
            var energy = ServiceLocator.Get<Energy>();

            var deck = _runData.Deck;
            _handController.Deck = deck;
            deckView.Setup(deck);
            _handController.Deck.MergeAndShuffle();

            energy.Initialize();
            energy.Reset();

            _handController.gameObject.SetActive(true);
            _handController.DrawHand();
            //_handController.DiscardHand();
        }

        public void OnCombatEnd()
        {
            _combatManager.EndCombat();
            _handController.DiscardHand();
            OfferCardReward();
        }
    }
}