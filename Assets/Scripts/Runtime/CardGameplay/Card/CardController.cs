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
        public event Action<CardController> OnSelectionCancled;

        public CardInstance Instance { get; private set; }
        public CardView View { get; set; }

        [Button]
        public void Init(CardData data, int number, Suit suit)
        {
            Number = number;
            Suit = suit;
            _selectStrategy = data.SelectStrategy;
            _playStrategy = data.PlayStrategy;

            Instance = new CardInstance(data, number);
            View = GetComponent<CardView>();
        }

        public void Init(CardInstance instance)
        {
            Init(instance.data, instance.number, instance.Suit);
        }


        public void Select(CardSelectStrategy selectStrategy)
        {
            //If the card is not selectable, return
            if (!Selectable)
            {
                return;
            }

            OnSelectionStart?.Invoke(this);
            //handle card selection

            if (HandController.Instance.Has(this))
            {
                if (!selectStrategy.Select(this)) return;
                MoveToBoard();
            }
            else
            {
                //else, add it to the hand
                MoveCardFromBoardToHand();
            }
        }

        private static void MoveCardFromBoardToHand()
        {
            var card = BoardController.Instance.Remove();
            HandController.Instance.AddCard(card);
        }

        private void MoveToBoard()
        {
            if (!BoardController.Instance.AddToSequence(this))
            {
                OnSelectionCancled?.Invoke(this);
                return;
            }

            HandController.Instance.RemoveCard(this);
        }

        public void Play()
        {
            _playStrategy.Play(this);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            Select();
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
        Blue,
        Purple,
        Yellow,
        Green,
        Black,
        White,
        Defualt
    }
}