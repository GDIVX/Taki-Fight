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
    /// Handle The behaviour of a card
    /// </summary>
    public class CardController : MonoBehaviour, IPointerClickHandler
    {
        public int Rank { get; private set; }
        public Suit Suit { get; private set; }

        //TODO: hook some visual indicator
        public bool Selectable { get; set; } = true;

        [ShowInInspector, ReadOnly] private CardSelectStrategy _selectStrategy;
        [ShowInInspector, ReadOnly] private CardPlayStrategy _playStrategy;

        public event Action<CardController> OnSelectionStart;
        public event Action<CardController> OnSelectionCanceled;

        public CardInstance instance;
        public CardView View { get; private set; }
        public float PlayDuration => _playStrategy?.Duration ?? 0f;
        public int EnergyCost { get; set; }

        [Button]
        public void Init(CardData data, int rank, Suit suit)
        {
            if (data == null)
            {
                Debug.LogError("CardData cannot be null during initialization.");
                return;
            }

            Rank = rank;
            Suit = suit;
            _selectStrategy = data.SelectStrategy;
            _playStrategy = data.PlayStrategy;
            EnergyCost = data.EnergyCost;

            instance = new CardInstance(data, rank);
            View = GetComponent<CardView>();
            instance.Controller = this;
        }

        public void Init(CardInstance cardInstance)
        {
            if (cardInstance == null)
            {
                Debug.LogError("CardInstance cannot be null during initialization.");
                return;
            }

            Init(cardInstance.data, cardInstance.number, cardInstance.Suit);
        }

        private async void Select(CardSelectStrategy selectStrategy)
        {
            if (selectStrategy == null)
            {
                Debug.LogError("CardSelectStrategy cannot be null.");
                return;
            }

            if (!Selectable)
            {
                return;
            }

            OnSelectionStart?.Invoke(this);

            if (await HandleSelectionStrategy(selectStrategy)) return;

            TryToPlay();
        }

        private async Task<bool> HandleSelectionStrategy(CardSelectStrategy selectStrategy)
        {
            if (!HandController.Instance.Has(this)) return true;
            if (!await selectStrategy.SelectAsync(this))
            {
                OnSelectionCanceled?.Invoke(this);
                return true;
            }

            return false;
        }

        public void Play()
        {
            if (_playStrategy == null)
            {
                Debug.LogError("PlayStrategy is not set.");
                return;
            }

            _playStrategy.Play(GameManager.Instance.Hero);
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
            if (!BoardController.Instance.CanPlayCard(this))
            {
                OnSelectionCanceled?.Invoke(this);
                return;
            }

            BoardController.Instance.UpdateMatch(this);
            HandController.Instance.RemoveCard(this);
            Play();
        }

        private void Select()
        {
            Select(_selectStrategy);
        }

        /// <summary>
        /// Remove the card
        /// </summary>
        public void Disable()
        {
            CardFactory.Instance.Disable(this);
        }
    }

    public enum Suit
    {
        Red,
        Yellow,
        Green,
        Blue,
        Purple,
        Black,
        White,
        Default
    }
}