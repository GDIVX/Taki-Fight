using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Runtime.CardGameplay.Card.CardBehaviour;
using Runtime.CardGameplay.Card.View;
using Runtime.CardGameplay.Deck;
using Runtime.CardGameplay.ManaSystem;
using Runtime.Combat.Pawn;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using Utilities;

namespace Runtime.CardGameplay.Card
{
    /// <summary>
    /// Handle the behaviour of a card.
    /// </summary>
    public class CardController : MonoBehaviour, IPointerClickHandler
    {
        public CardType CardType { get; private set; }
        [ShowInInspector, ReadOnly] public TrackedProperty<bool> IsPlayable;

        [ShowInInspector, ReadOnly] private CardSelectStrategy _selectStrategy;
        [ShowInInspector, ReadOnly] private List<(CardPlayStrategy, int)> _playStrategies;
        [ShowInInspector, ReadOnly] private CardAffordabilityStrategy _affordabilityStrategy;
        [ShowInInspector, ReadOnly] private List<CardPostPlayStrategy> _postPlayStrategies;


        public static event Action<CardController> OnCardPlayedEvent;

        public CardInstance Instance { get; private set; }
        public Transform Transform => gameObject.transform;
        public CardView View { get; private set; }


        public CardData Data { get; private set; }

        public HandController HandController { get; private set; }
        public ManaInventory ManaInventory { get; private set; }
        public PawnController Pawn { get; private set; }
        public List<Mana> Cost => Instance.Cost;

        private CardFactory _cardFactory;

        [Button]
        public void Init(CardData data, CardDependencies dependencies)
        {
            if (data == null)
            {
                Debug.LogError("CardData cannot be null during initialization.");
                return;
            }

            HandController = dependencies.HandController;
            ManaInventory = dependencies.ManaInventory;
            Pawn = dependencies.Pawn;
            _cardFactory = dependencies.CardFactory;

            CardType = data.CardType;

            _selectStrategy = data.SelectStrategy;
            _playStrategies = CreatePlayStrategyTupletList(data.PlayStrategies);
            _affordabilityStrategy = data.AffordabilityStrategy;
            _postPlayStrategies = data.PostPlayStrategies;


            Instance = new CardInstance(data)
            {
                Controller = this
            };
            View = GetComponent<CardView>();

            //Card is selectable only when it can be played
            IsPlayable = new TrackedProperty<bool>()
            {
                Value = true
            };
            OnCardPlayedEvent += OnCardPlayed;
            Data = data;
        }

        private void OnCardPlayed(CardController c)
        {
            IsPlayable.Value = _affordabilityStrategy.CanPlayCard(this);
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

        public void Init(CardInstance cardInstance, CardDependencies dependencies)
        {
            if (cardInstance == null)
            {
                Debug.LogError("CardInstance cannot be null during initialization.");
                return;
            }

            Init(cardInstance.Data, dependencies);
        }

        private async void Select(CardSelectStrategy selectStrategy)
        {
            if (!IsPlayable.Value)
            {
                Debug.LogWarning($"Trying to select card {name} who is not selectable");
                return;
            }

            if (selectStrategy == null)
            {
                Debug.LogError("CardSelectStrategy cannot be null.");
                return;
            }


            if (await HandleSelectionStrategyAsync(selectStrategy))
            {
                TryToPlay();
            }
        }


        private async Task<bool> HandleSelectionStrategyAsync(CardSelectStrategy selectStrategy)
        {
            return HandController.Has(this) && await selectStrategy.SelectAsync(this);
        }

        private void Play()
        {
            StartCoroutine(HandlePlay());
        }

        private IEnumerator HandlePlay()
        {
            foreach (var tuple in _playStrategies)
            {
                tuple.Item1.Play(Pawn, tuple.Item2);
                yield return new WaitForSeconds(tuple.Item1.Duration);
            }

            foreach (var postPlayStrategy in _postPlayStrategies.Where(strategy => strategy != null))
            {
                postPlayStrategy.PostPlay(this);
            }

            OnCardPlayedEvent?.Invoke(this);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData == null)
            {
                Debug.LogWarning("OnPointerClick called with null eventData.");
                return;
            }

            Select();
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
            IsPlayable.Value = _affordabilityStrategy.CanPlayCard(this);
        }


        private void TryToPlay()
        {
            if (!IsPlayable.Value)
            {
                return;
            }


            Play();
        }


        private void Select()
        {
            Select(_selectStrategy);
        }
    }
}