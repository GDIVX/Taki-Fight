using System;
using System.Collections.Generic;
using Runtime.CardGameplay.Card.CardBehaviour;
using Runtime.CardGameplay.Card.CardBehaviour.Feedback;
using Runtime.CardGameplay.Card.View;
using Runtime.CardGameplay.Deck;
using Runtime.Selection;
using Runtime.UI;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using Utilities;

namespace Runtime.CardGameplay.Card
{
    public class CardController : MonoBehaviour, IPointerClickHandler, ISelectableEntity
    {
        [ShowInInspector, ReadOnly] public TrackedProperty<bool> IsPlayable;

        private CardFactory _cardFactory;
        [ShowInInspector] [ReadOnly] private FeedbackStrategy _feedbackStrategy;
        private bool _isSelecting;

        [ShowInInspector, ReadOnly] private List<(CardPlayStrategy, int)> _playStrategies;
        public CardType CardType { get; private set; }

        public CardInstance Instance { get; private set; }
        public Transform Transform => gameObject.transform;
        public CardView View { get; private set; }

        public CardData Data { get; private set; }

        public HandController HandController { get; private set; }
        public Energy.Energy Energy { get; private set; }
        public int Cost => Instance.Cost;

        public void OnPointerClick(PointerEventData eventData)
        {
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
            if (data == null)
            {
                Debug.LogError("CardData cannot be null during initialization.");
                return;
            }

            Instance = new CardInstance(data)
            {
                Controller = this
            };

            CardType = data.CardType;

            _feedbackStrategy = data.FeedbackStrategy;
            _playStrategies = CreatePlayStrategyTupletList(data.PlayStrategies);

            _cardFactory = ServiceLocator.Get<CardFactory>();
            Energy = ServiceLocator.Get<Energy.Energy>();
            HandController = ServiceLocator.Get<HandController>();

            View = GetComponent<CardView>();

            IsPlayable = new TrackedProperty<bool>(true);
            OnCardPlayedEvent += _ => UpdateAffordability();
            Energy.OnAmountChanged += _ => UpdateAffordability();
            Data = data;

            // SelectionService.Instance.Register(this);
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
            return _playStrategies[index].Item2;
        }

        public void SetPotency(int index, int newValue)
        {
            (CardPlayStrategy, int) tuple = _playStrategies[index];
            _playStrategies.Remove(tuple);
            tuple.Item2 = newValue;
            _playStrategies.Insert(index, tuple);
        }


        private void Play()
        {
            if (!CanAfford())
            {
                var bannerView = ServiceLocator.Get<BannerViewManager>();
                bannerView.WriteMessage(1, "Can't Afford to Play This Card", Color.red);
                this.Timer(1f, () => bannerView.Clear());
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

        [Button]
        private void RunPlayLogic()
        {
            int remainingPlays = _playStrategies.Count;


            foreach (var tuple in _playStrategies)
            {
                tuple.Item1.Play(this, tuple.Item2, OnStrategyComplete);
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

            if (Data.DestroyCardAfterUse)
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
            _cardFactory.Disable(this);
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
    }

    [Serializable]
    public struct GemGroup
    {
        public int Pearls;
        public int Quartz;
        public int Brimstone;
    }
}