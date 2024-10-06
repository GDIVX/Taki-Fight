using System;
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
        public int Number { get; private set; }
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

        [Button]
        public void Init(CardData data, int number, Suit suit)
        {
            if (data == null)
            {
                Debug.LogError("CardData cannot be null during initialization.");
                return;
            }

            Number = number;
            Suit = suit;
            _selectStrategy = data.SelectStrategy;
            _playStrategy = data.PlayStrategy;

            instance = new CardInstance(data, number);
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

            // If the card is not selectable, return
            if (!Selectable)
            {
                return;
            }

            OnSelectionStart?.Invoke(this);

            // Handle card selection
            if (HandController.Instance.Has(this))
            {
                if (!await selectStrategy.SelectAsync(this))
                {
                    OnSelectionCanceled?.Invoke(this);
                    return;
                }

                MoveToBoard();
            }
            else
            {
                // Else, add it to the hand
                MoveCardFromBoardToHand();
            }
        }

        public void Play()
        {
            if (_playStrategy == null)
            {
                Debug.LogError("PlayStrategy is not set.");
                return;
            }

            _playStrategy.Play(this);
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

        private static void MoveCardFromBoardToHand()
        {
            var card = BoardController.Instance.Remove();
            if (card != null)
            {
                HandController.Instance.AddCard(card);
            }
        }

        private void MoveToBoard()
        {
            if (!BoardController.Instance.AddToSequence(this))
            {
                OnSelectionCanceled?.Invoke(this);
                return;
            }

            HandController.Instance.RemoveCard(this);
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