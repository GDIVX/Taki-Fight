using System;
using System.Collections.Generic;
using System.Linq;
using Runtime.CardGameplay.Card.CardBehaviour;
using Runtime.CardGameplay.Card.CardBehaviour.Feedback;
using Runtime.CardGameplay.Card.View;
using Runtime.CardGameplay.Deck;
using Runtime.Combat.Tilemap;
using Runtime.Selection;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using Utilities;

namespace Runtime.CardGameplay.Card
{
    public sealed class CardController : MonoBehaviour, IPointerClickHandler, ISelectableEntity, ICloneable
    {
        [ShowInInspector, ReadOnly] public Observable<bool> IsPlayable;

        private CardFactory _cardFactory;
        private FeedbackStrategy _feedbackStrategy;
        private bool _isSelecting;
        private CardViewMediator _viewMediator;

        [ShowInInspector, ReadOnly] private List<PlayStrategyData> _playStrategies;
        [SerializeField] private CardView _view;

        public List<PlayStrategyData> PlayStrategies
        {
            get => _playStrategies;
            internal set => _playStrategies = value;
        }

        public CardType CardType { get; private set; }
        public CardInstance Instance { get; private set; }
        public Transform Transform => gameObject.transform;

        internal CardView View => _view;

        public CardViewMediator ViewMediator => _viewMediator;
        public CardData Data { get; private set; }
        public bool IsConsumed { get; set; }
        public HandController HandController { get; private set; }
        public Energy.Energy Energy { get; private set; }

        public event Action<CardController> OnRetained;

        public int Cost
        {
            get => Instance.Cost;
            set
            {
                Instance.Cost = value;
                _viewMediator?.Refresh();
            }
        }


        public void OnPointerClick(PointerEventData eventData)
        {
            if (!HandController.Has(this))
            {
                Debug.LogWarning("Trying to play a card that is not in hand.");
                return;
            }

            if (eventData == null)
            {
                Debug.LogWarning("OnPointerClick called with null eventData.");
                return;
            }

            if (eventData.button != PointerEventData.InputButton.Left) return;

            if (SelectionService.Instance.CurrentState == SelectionState.InProgress)
            {
                TryToSelect();
            }
            else
            {
                TryToPlay();
            }
        }

        public void TryToSelect()
        {
            if (SelectionService.Instance.CurrentState != SelectionState.InProgress) return;

            var predicate = SelectionService.Instance.Predicate;
            if (predicate.Invoke(this)) SelectionService.Instance.Select(this);
            else SelectionService.Instance.Cancel();
        }

        public void OnSelected()
        {
        }

        public void OnDeselected()
        {
        }

        public static event Action<CardController> OnCardPlayedEvent;

        [Button]
        public void Init(CardData data, CardDependencies dependencies)
        {
            if (!data)
            {
                Debug.LogError("CardData cannot be null during initialization.");
                return;
            }

            CardInstance cardInstance = new CardInstance(data);
            Init(cardInstance);
        }

        public bool Init(CardInstance instance)
        {
            if (instance == null)
            {
                Debug.LogError("CardInstance cannot be null during initialization.");
                return false;
            }

            if (instance.State == CardInstance.CardInstanceState.Hand)
            {
                Debug.LogWarning($"CardInstance {instance.Guid} state is already hand. Disabling duplicates.");
                gameObject.SetActive(false);
                return false;
            }

            Instance = instance;
            Instance.Controller = this;
            CardType = instance.Data.CardType;
            Data = instance.Data;

            _feedbackStrategy = instance.Data.FeedbackStrategy;


            _playStrategies = new List<PlayStrategyData>(instance.Data.PlayStrategies);
            _playStrategies.ForEach(s => s.PlayStrategy.Initialize(s, this));

            _cardFactory = ServiceLocator.Get<CardFactory>();
            Energy = ServiceLocator.Get<Energy.Energy>();
            HandController = ServiceLocator.Get<HandController>();

            IsPlayable = new Observable<bool>(true);
            IsConsumed = instance.Data.IsConsumed;

            OnCardPlayedEvent += _ => UpdateAffordability();
            Energy.OnAmountChanged += _ => UpdateAffordability();

            _viewMediator = new CardViewMediator();
            _viewMediator.Bind(this, View);
            gameObject.name = instance.Data.Title + instance.Guid;

            return true;
        }

        private void UpdateAffordability()
        {
            IsPlayable.Value = CanAfford();
            _viewMediator?.Refresh();
        }

        private void TryToPlay()
        {
            if (!IsPlayable.Value)
            {
                _viewMediator?.ShowMessage(!CanAfford()
                    ? $"Need {Cost} Mana, but I only have {Energy.Amount}"
                    : "Can't Play");

                return;
            }

            Play();
        }

        private void Play()
        {
            if (_feedbackStrategy)
            {
                _feedbackStrategy.Animate(() => RunPlayLogic());
            }
            else
            {
                RunPlayLogic();
            }
        }

        [Button]
        internal void RunPlayLogic(bool blind = false)
        {
            int remainingPlays = _playStrategies.Count;

            foreach (var strategy in _playStrategies)
            {
                if (blind)
                {
                    strategy.PlayStrategy.BlindPlay(this, OnStrategyComplete);
                }
                else
                {
                    strategy.PlayStrategy.Play(this, OnStrategyComplete);
                }
            }

            void OnStrategyComplete(bool isResolved)
            {
                if (!isResolved) return;
                remainingPlays--;
                if (remainingPlays <= 0)
                {
                    HandlePostPlay();
                }
            }
        }

        private void HandlePostPlay()
        {
            HandleCost();

            if (IsConsumed)
                HandController.ConsumeCard(this);
            else
                HandController.DiscardCard(this);

            OnCardPlayedEvent?.Invoke(this);
        }

        private void HandleCost()
        {
            Energy.Remove(Cost);
        }

        private bool CanAfford()
        {
            return Energy.Amount >= Cost;
        }

        public void OnDiscard()
        {
            _viewMediator.Consume();
        }

        public void Disable()
        {
            _cardFactory.ReturnToPool(this);
        }

        public void OnDraw()
        {
            UpdateAffordability();
            _viewMediator.Draw();
        }

        public void Discard()
        {
            HandController?.DiscardCard(this);
            _viewMediator.Discard();
        }

        public void Consume()
        {
            HandController?.ConsumeCard(this);
        }

        public void Limbo()
        {
            HandController?.LimboCard(this);
        }

        public void InvokeOnRetained()
        {
            OnRetained?.Invoke(this);
        }

        public object Clone()
        {
            var clone = _cardFactory.Create(Instance.Clone() as CardInstance);
            clone._playStrategies = new List<PlayStrategyData>(_playStrategies);
            return clone;
        }

        public void AddStrategies(List<PlayStrategyData> newStrategies)
        {
            _playStrategies.AddRange(newStrategies);
        }

        internal bool IsAskingForCard(CardController otherCard)
        {
            return _playStrategies.Any(data => data.PlayStrategy.IsAskingForCard(otherCard));
        }

        internal bool IsAskingForTile(Tile tile)
        {
            return _playStrategies.Any(data => data.PlayStrategy.IsValidTile(tile));
        }
    }
}