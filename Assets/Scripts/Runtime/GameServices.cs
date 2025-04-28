using Runtime.CardGameplay.Card;
using Runtime.CardGameplay.Energy;
using Runtime.Combat;
using Runtime.Rewards;
using Runtime.UI;
using Runtime.UI.Tooltip;
using Runtime.Events;
using UnityEngine;
using Sirenix.OdinInspector;
using Runtime.CardGameplay.Deck;
using Runtime.RunManagement;
using Utilities;
using Runtime.Combat.Pawn;

namespace Runtime
{
    public class GameServices : MonoBehaviour
    {
        [SerializeField, Required] private Energy _energy;
        [SerializeField, Required] private HandController _handController;
        [SerializeField, Required] private DeckView _deckView;
        [SerializeField, Required] private CardFactory _cardFactory;
        [SerializeField, Required] private PawnFactory _pawnFactory;
        [SerializeField, Required] private RewardsOfferController _rewardsOfferController;
        [SerializeField, Required] private BannerViewManager _bannerViewManager;
        [SerializeField, Required] private CombatManager _combatManager;
        [SerializeField] private TooltipPool _tooltipPool;
        [SerializeField] private KeywordDictionary _keywordDictionary;

        private void OnValidate()
        {
            if (_energy == null) _energy = FindFirstObjectByType<Energy>();
            if (_handController == null) _handController = FindFirstObjectByType<HandController>();
            if (_deckView == null) _deckView = FindFirstObjectByType<DeckView>();
            if (_cardFactory == null) _cardFactory = FindFirstObjectByType<CardFactory>();
            if (_pawnFactory == null) _pawnFactory = FindFirstObjectByType<PawnFactory>();
            if (_rewardsOfferController == null) _rewardsOfferController = FindFirstObjectByType<RewardsOfferController>();
            if (_bannerViewManager == null) _bannerViewManager = FindFirstObjectByType<BannerViewManager>();
            if (_combatManager == null) _combatManager = FindFirstObjectByType<CombatManager>();
            if (_tooltipPool == null) _tooltipPool = FindFirstObjectByType<TooltipPool>();
            if (_keywordDictionary == null) _keywordDictionary = FindFirstObjectByType<KeywordDictionary>();
        }

        private void Awake()
        {
            ServiceLocator.Register(_energy);
            ServiceLocator.Register(_cardFactory);
            ServiceLocator.Register(_handController);
            ServiceLocator.Register(_deckView);
            ServiceLocator.Register(_rewardsOfferController);
            ServiceLocator.Register(_bannerViewManager);
            ServiceLocator.Register(_combatManager);
            ServiceLocator.Register(_tooltipPool);
            ServiceLocator.Register(_keywordDictionary);
            ServiceLocator.Register(_pawnFactory);
        }
    }
}



