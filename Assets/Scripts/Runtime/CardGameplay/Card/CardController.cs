using System;
using System.Threading.Tasks;
using Runtime.CardGameplay.Board;
using Runtime.CardGameplay.Card.CardBehaviour;
using Runtime.Combat.Pawn;
using Runtime.Events;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using Utilities;

namespace Runtime.CardGameplay.Card
{
    /// <summary>
    /// Handle the behaviour of a card.
    /// </summary>
    public class CardController : MonoBehaviour, IPointerClickHandler, ICardController
    {
        public int Rank { get; set; }
        public Suit Suit { get; set; }
        public int Potency { get; private set; }
        public CardType CardType { get; private set; }
        [ShowInInspector, ReadOnly] public TrackedProperty<bool> IsPlayable;

        [ShowInInspector, ReadOnly] private CardSelectStrategy _selectStrategy;
        [ShowInInspector, ReadOnly] private CardPlayStrategy _playStrategy;


        public static event Action<CardController> OnCardPlayed;

        public CardInstance Instance { get; private set; }
        public Transform Transform => gameObject.transform;
        public CardView View { get; private set; }
        public CardData Data { get; private set; }
        public float PlayDuration => _playStrategy?.Duration ?? 0f;
        public int EnergyCost { get; set; }

        private IHandController _handController;
        private IBoardController _boardController;
        private PawnController _pawn;
        private ICardFactory _cardFactory;

        [Button]
        public void Init(CardData data, int rank, Suit suit, CardDependencies dependencies)
        {
            if (data == null)
            {
                Debug.LogError("CardData cannot be null during initialization.");
                return;
            }

            _handController = dependencies.HandController;
            _boardController = dependencies.BoardController;
            _pawn = dependencies.Pawn;
            _cardFactory = dependencies.CardFactory;

            Rank = rank;
            Suit = suit;
            Potency = data.Potency;
            CardType = data.CardType;
            _selectStrategy = data.SelectStrategy;
            _playStrategy = data.PlayStrategy;
            EnergyCost = data.EnergyCost;

            Instance = new CardInstance(data, rank)
            {
                Controller = this
            };
            View = GetComponent<CardView>();

            //Card is selectable only when it can be played
            IsPlayable = new TrackedProperty<bool>()
            {
                Value = true
            };
            OnCardPlayed += (c) => { IsPlayable.Value = CanPlayCard(); };
            Data = data;
        }


        public void Init(CardInstance cardInstance, CardDependencies dependencies)
        {
            if (cardInstance == null)
            {
                Debug.LogError("CardInstance cannot be null during initialization.");
                return;
            }

            Init(cardInstance.Data, cardInstance.Rank, cardInstance.Suit, dependencies);
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
            return _handController.Has(this) && await selectStrategy.SelectAsync(this);
        }

        private void Play()
        {
            if (_playStrategy == null)
            {
                Debug.LogError("PlayStrategy is not set.");
                return;
            }

            //Play the card before discarding it or updating the current suit and rank.
            //In order to preserve the game state for any play strategy before doing changes 
            _playStrategy.Play(_pawn, Potency);
            _playStrategy.PostPlay(_boardController, _handController, this);
            OnCardPlayed?.Invoke(this);
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

        public bool CanPlayCard()
        {
            return _boardController.CanPlayCard(this);
        }

        private void TryToPlay()
        {
            if (!CanPlayCard())
            {
                return;
            }


            Play();
        }


        private void Select()
        {
            Select(_selectStrategy);
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
            IsPlayable.Value = CanPlayCard();
        }
    }
}