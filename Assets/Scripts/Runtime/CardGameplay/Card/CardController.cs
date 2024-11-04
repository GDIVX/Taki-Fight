using System;
using System.Threading.Tasks;
using Runtime.CardGameplay.Board;
using Runtime.CardGameplay.Card.CardBehaviour;
using Runtime.CardGameplay.Deck;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Runtime.CardGameplay.Card
{
    /// <summary>
    /// Handle the behaviour of a card.
    /// </summary>
    public class CardController : MonoBehaviour, IPointerClickHandler
    {
        public int Rank { get; private set; }
        public Suit Suit { get; private set; }
        public bool Selectable { get; set; } = true;

        [ShowInInspector, ReadOnly] private CardSelectStrategy _selectStrategy;
        [ShowInInspector, ReadOnly] private CardPlayStrategy _playStrategy;

        public event Action<CardController> OnSelectionStart;
        public event Action<CardController> OnSelectionCanceled;

        public CardInstance Instance { get; private set; }
        public CardView View { get; private set; }
        public float PlayDuration => _playStrategy?.Duration ?? 0f;
        public int EnergyCost { get; private set; }

        private IHandController _handController;
        private IBoardController _boardController;
        private IGameManager _gameManager;
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
            _gameManager = dependencies.GameManager;
            _cardFactory = dependencies.CardFactory;

            Rank = rank;
            Suit = suit;
            _selectStrategy = data.SelectStrategy;
            _playStrategy = data.PlayStrategy;
            EnergyCost = data.EnergyCost;

            Instance = new CardInstance(data, rank);
            View = GetComponent<CardView>();
            Instance.Controller = this;
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
            if (selectStrategy == null || !Selectable)
            {
                Debug.LogError("CardSelectStrategy cannot be null or the card is not selectable.");
                return;
            }

            OnSelectionStart?.Invoke(this);

            if (!await HandleSelectionStrategy(selectStrategy))
            {
                OnSelectionCanceled?.Invoke(this);
            }
            else
            {
                TryToPlay();
            }
        }

        private async Task<bool> HandleSelectionStrategy(CardSelectStrategy selectStrategy)
        {
            if (!_handController.Has(this)) return false;
            return await selectStrategy.SelectAsync(this);
        }

        private void Play()
        {
            if (_playStrategy == null)
            {
                Debug.LogError("PlayStrategy is not set.");
                return;
            }

            _playStrategy.Play(_gameManager.Hero);
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

        private void TryToPlay()
        {
            if (!_boardController.CanPlayCard(this))
            {
                OnSelectionCanceled?.Invoke(this);
                return;
            }

            _boardController.UpdateMatch(this);
            _handController.DiscardCard(this);
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
    }
}