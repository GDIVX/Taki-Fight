using Runtime.CardGameplay.Card.CardBehaviour;
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
        public bool Selectable { get; set; }

        [ShowInInspector, ReadOnly] private CardSelectStrategy _selectStrategy;
        [ShowInInspector, ReadOnly] private CardPlayStrategy _playStrategy;

        public void Init(CardData data, int number)
        {
            Number = number;
            Suit = data.Suit;
        }

        public void Select()
        {
            //If the card is not selectable, return
            if (!Selectable) return;

            //handle card selection
            if (_selectStrategy.Select(this))
            {
                //if valid, add it to the sequence
                //Table.Instance.AddToSequence(this);
                //Hand.Instance.Remove(this);
            }
            else
            {
                //else, add it to the hand
                //Hand.Instance.Add(this);
                //Table.Instance.Remove(this);
            }
        }

        public void Play()
        {
            _playStrategy.Play(this);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            Select();
        }
    }

    public enum Suit
    {
        Red,
        Blue,
        Purple,
        Yellow,
        Green
    }
}