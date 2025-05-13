using System;
using Runtime.CardGameplay.Card;
using Runtime.CardGameplay.Deck;
using Runtime.CardGameplay.Energy;
using Runtime.Combat;
using Runtime.Rewards;
using Runtime.RunManagement;
using Runtime.UI;
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


        //TODO: TEMP
        [SerializeField] private PlayerClassData _tempClassData;

        private static CardFactory CardFactory => ServiceLocator.Get<CardFactory>();
        private static CombatManager CombatManager => ServiceLocator.Get<CombatManager>();
        private static HandController HandController => ServiceLocator.Get<HandController>();
        private static RewardsOfferController RewardsOfferController => ServiceLocator.Get<RewardsOfferController>();

        public RunBuilder RunBuilder { get; private set; }
        public EventBus EventBus { get; private set; }

        private void Awake()
        {
            EventBus = new EventBus();
            ServiceLocator.Register(EventBus);
            RunBuilder = new RunBuilder(_runData);
            ServiceLocator.Register(RunBuilder);
            OnEventBusCreated?.Invoke();
        }


        public event Action OnEventBusCreated;


        private void OfferCardReward()
        {
            RewardsOfferController.OfferRewards(() =>
            {
                //TODO: replace with exploration and progression
                CombatManager.StartCombat();
            });
        }

        /// <summary>
        /// We are going to use the currently saved run. For a new run, create a new file
        /// </summary>
        public void StartRun()
        {
            //TODO: TEMP
            RunBuilder.NewRunFromPlayerClass(_tempClassData);

            CardFactory.Init();
            RewardsOfferController.Init();
            CombatManager.Init();
        }

        private void GameOver()
        {
            HandController.gameObject.SetActive(false);
            _newGameButtonObject.SetActive(true);
        }

        [Button]
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
            //_handController.DiscardHand();
        }

        private void OnCombatEnd()
        {
            CombatManager.EndCombat();
            HandController.DiscardHand();
        }

        public void WinCombat()
        {
            OnCombatEnd();
            OfferCardReward();
        }

        public void EndRun()
        {
            OnCombatEnd();
            GameOver();

            //tempt game over message
            ServiceLocator.Get<BannerViewManager>().WriteMessage(0, "Game Over", Color.red);
        }
    }
}