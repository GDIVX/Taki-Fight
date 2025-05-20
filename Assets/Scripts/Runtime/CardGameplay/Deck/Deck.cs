using System;
using System.Collections.Generic;
using System.Linq;
using Runtime.CardGameplay.Card;
using Runtime.UI.OnScreenMessages;
using Sirenix.OdinInspector;
using Utilities;

namespace Runtime.CardGameplay.Deck
{
    [Serializable]
    public class Deck
    {
        [ShowInInspector, ReadOnly, TableList] private Stack<CardInstance> _consumePile;

        [ShowInInspector] [ReadOnly] [TableList]
        private Stack<CardInstance> _discardPile;

        [ShowInInspector] [ReadOnly] [TableList]
        private Stack<CardInstance> _drawPile;

        public Deck(List<CardInstance> cards)
        {
            Setup(cards);
        }

        public event Action<Stack<CardInstance>> OnDrawPileUpdated;
        public event Action<Stack<CardInstance>> OnDiscardPileUpdated;
        public event Action<Stack<CardInstance>> OnBurnPileUpdated;

        private void Setup(List<CardInstance> cards)
        {
            _drawPile = new Stack<CardInstance>(cards);
            _discardPile = new Stack<CardInstance>();
            _consumePile = new Stack<CardInstance>();
            MergeAndShuffle();

            OnDrawPileUpdated?.Invoke(_drawPile);
            OnDiscardPileUpdated?.Invoke(_discardPile);
            OnBurnPileUpdated?.Invoke(_consumePile);
        }

        public bool Draw(out CardInstance cardInstance)
        {
            if (_drawPile.Count == 0)
            {
                if (_discardPile.Count == 0)
                {
                    ServiceLocator.Get<MessageManager>().ShowMessage("Can't Draw More Cards", MessageType.Notification);
                    cardInstance = null;
                    return false;
                }

                MergePiles();
            }

            cardInstance = _drawPile.Pop();
            OnDrawPileUpdated?.Invoke(_drawPile);
            return true;
        }

        public bool TryToFindAndRemoveCard(CardData cardData, out CardInstance cardInstance)
        {
            return TryToFindAndRemoveCard(card => card.Data.Title == cardData.Title, out cardInstance);
        }

        public bool TryToFindAndRemoveCard(Func<CardInstance, bool> predicate, out CardInstance cardInstance)
        {
            cardInstance = null;

            if (!_drawPile.Any(predicate)) return false;

            var listedPile = _drawPile.ToList();
            cardInstance = listedPile.First(predicate);
            listedPile.Remove(cardInstance);

            // Rebuild the stack
            _drawPile = new Stack<CardInstance>(listedPile);

            OnDrawPileUpdated?.Invoke(_drawPile);
            return true;
        }

        public void Discard(CardInstance card)
        {
            _discardPile.Push(card);
            OnDiscardPileUpdated?.Invoke(_discardPile);
        }

        public void Consume(CardInstance card)
        {
            _consumePile.Push(card);
            OnBurnPileUpdated?.Invoke(_consumePile);
        }

        public void MergeAndShuffle()
        {
            MergePiles();
            var cards = ShuffleCards();

            // Clear the draw pile and add the shuffled cards back
            _drawPile.Clear();
            foreach (var card in cards)
            {
                _drawPile.Push(card);
            }

            OnDrawPileUpdated?.Invoke(_drawPile);
        }

        private List<CardInstance> ShuffleCards()
        {
            var cards = new List<CardInstance>(_drawPile);
            cards.Shuffle();
            return cards;
        }

        private void MergePiles()
        {
            //merge discard pile
            while (_discardPile.Count > 0)
            {
                _drawPile.Push(_discardPile.Pop());
            }
        }

        public bool IsDiscarded(CardInstance card)
        {
            return _discardPile.Contains(card);
        }

        public bool IsInDrawPile(CardInstance card)
        {
            return _drawPile.Contains(card);
        }

        public bool Exist(CardInstance card)
        {
            return IsDiscarded(card) || IsInDrawPile(card);
        }

        public bool IsConsumed(CardInstance card)
        {
            return _consumePile.Contains(card);
        }
    }
}