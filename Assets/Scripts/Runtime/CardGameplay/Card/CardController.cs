using System;
using System.Collections.Generic;
using Runtime.CardGameplay.Card.CardBehaviour;
using Runtime.CardGameplay.Card.CardBehaviour.Feedback;
using Runtime.CardGameplay.Card.View;
using Runtime.CardGameplay.Deck;
using Runtime.Selection;
using Runtime.UI.OnScreenMessages;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using Utilities;

namespace Runtime.CardGameplay.Card
{
    public class CardController : MonoBehaviour, IPointerClickHandler, ISelectableEntity, ICloneable
    {
        [ShowInInspector] [ReadOnly] public Observable<bool> IsPlayable;

        private CardFactory _cardFactory;
        [ShowInInspector] [ReadOnly] private FeedbackStrategy _feedbackStrategy;
        private bool _isSelecting;

        [ShowInInspector] [ReadOnly] private List<PlayStrategyData> _playStrategies;

        public List<PlayStrategyData> PlayStrategies
        {
            get => _playStrategies;
            internal set => _playStrategies = value;
        }

        public CardType CardType { get; private set; }

        public CardInstance Instance { get; private set; }
        public Transform Transform => gameObject.transform;
        public CardView View { get; set; }

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
                View?.SetCost(value);
                View?.UpdateDescription();
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
                // If selection is in progress, validate card selection instead of playing it
                TryToSelect();
            else
                // Normal card play logic when selection is NOT active
                TryToPlay();
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

            _feedbackStrategy = instance.Data.FeedbackStrategy;
            View = GetComponent<CardView>();
            _playStrategies = new List<PlayStrategyData>(instance.Data.PlayStrategies);
            _playStrategies.ForEach(s => s.PlayStrategy.Initialize(s, this));

            _cardFactory = ServiceLocator.Get<CardFactory>();
            Energy = ServiceLocator.Get<Energy.Energy>();
            HandController = ServiceLocator.Get<HandController>();


            IsPlayable = new Observable<bool>(true);
            IsConsumed = instance.Data.IsConsumed;
            OnCardPlayedEvent += _ => UpdateAffordability();
            Energy.OnAmountChanged += _ => UpdateAffordability();
            Data = instance.Data;

            gameObject.name = instance.Data.Title + instance.Guid;

            return true;
        }

        private void UpdateAffordability()
        {
            IsPlayable.Value = CanAfford();
            View.UpdateDescription();
        }

        private List<(CardPlayStrategy, int)> CreatePlayStrategyTupletList(List<PlayStrategyData> dataPlayStrategies)
        {
            List<(CardPlayStrategy, int)> tuples = new();
            dataPlayStrategies.ForEach(x => tuples.Add((x.PlayStrategy, x.Potency)));
            return tuples;
        }

        public int GetPotency(int index)
        {
            return _playStrategies[index].Potency;
        }

        public void SetPotency(int index, int newValue)
        {
            var strategy = _playStrategies[index].PlayStrategy;
            strategy.Potency = newValue;
            View?.UpdateDescription();
        }


        public void Play()
        {
            if (!CanAfford())
            {
                ServiceLocator.Get<MessageManager>()
                    .ShowMessage("Not enough energy to play this card.", MessageType.Critical);
                return;
            }

            if (_feedbackStrategy)
            {
                //TODO: First play the card and then animate
                _feedbackStrategy.Animate(RunPlayLogic);
            }
            else
            {
                RunPlayLogic();
            }
        }

        private void RunPlayLogic()
        {
            RunPlayLogic(false);
        }

        [Button]
        internal void RunPlayLogic(bool blind = false)
        {
            int remainingPlays = _playStrategies.Count;


            foreach (var tuple in _playStrategies)
            {
                if (blind)
                {
                    tuple.PlayStrategy.BlindPlay(this, OnStrategyComplete);
                }
                else
                {
                    tuple.PlayStrategy.Play(this, OnStrategyComplete);
                }
            }

            return;

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
            {
                HandController.ConsumeCard(this);
            }
            else
            {
                HandController.DiscardCard(this);
            }

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
        }

        /// <summary>
        /// Remove the card.
        /// </summary>
        public void Disable()
        {
            _cardFactory.ReturnToPool(this);
        }

        public void OnDraw()
        {
            UpdateAffordability();
        }

        private void TryToPlay()
        {
            if (!IsPlayable.Value)
            {
                return;
            }

            Play();
        }

        public void Discard()
        {
            var hand = ServiceLocator.Get<HandController>();
            if (hand) hand.DiscardCard(this);
        }

        public void Consume()
        {
            var hand = ServiceLocator.Get<HandController>();
            if (hand) hand.ConsumeCard(this);
        }

        public void Limbo()
        {
            var hand = ServiceLocator.Get<HandController>();
            if (hand) hand.LimboCard(this);
        }

        public virtual void InvokeOnRetained()
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
    }
}