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

        [SerializeField, TabGroup("Dependencies"), Required]
        private BannerViewManager _bannerViewManager;

        [SerializeField, TabGroup("Dependencies"), Required]
        private CombatManager _combatManager;


        [SerializeField, TabGroup("Tempt")] private GameObject _newGameButtonObject;

        private EventBus _eventBus;
        [SerializeField] private TooltipPool _tooltipPool;
        [SerializeField] private KeywordDictionary _keywordDictionary;


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

        private void Awake()
        {
            _eventBus = new EventBus();
        }


        [Button]
        public void StartSession(PawnData data)
        {
            _cardCollection.Init();
            CombatManager.InitializeHero(data);
            CombatManager.SetupOnEnemyRemove();
            _cardFactory.Init(CombatManager.Hero);
        }


        public void GameOver()
        {
            CombatManager.EndCombat();
            _handController.gameObject.SetActive(false);
            _newGameButtonObject.SetActive(true);
        }

        [Button]
        public void SetupCardGameplay()
        {
            _handController.gameObject.SetActive(true);
            _handController.DiscardHand();
            _cardCollection.CreateDeck();
            _gemsBag.Initialize();
            CombatManager.StartTurn();
        }
    }
}