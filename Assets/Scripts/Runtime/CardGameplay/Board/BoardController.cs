using System;
using Runtime.CardGameplay.Card;
using Sirenix.OdinInspector;
using Utilities;

namespace Runtime.CardGameplay.Board
{
    public class BoardController : Utilities.Singleton<BoardController>
    {
        [ShowInInspector, ReadOnly] private bool _isLastCardMatch;

        [ShowInInspector, ReadOnly] private Suit CurrentSuit { get; set; }
        [ShowInInspector, ReadOnly] private int CurrentRank { get; set; }

        private readonly TrackedProperty<int> _matchCount = new();


        public event Action<Suit, int> OnMatchValuesChanged;

        private bool IsLastCardMatch
        {
            get => _isLastCardMatch;
            set => _isLastCardMatch = value;
        }


        public void UpdateMatch(CardController cardController)
        {
            UpdateCurrentSuitAndRank(cardController.Suit, cardController.Rank);
            _matchCount.Value += 1;
        }

        public void RegisterToMatchCountChanged(Action<int> action)
        {
            _matchCount.OnValueChanged += action;
        }

        public void UnregisterToMatchCountChanged(Action<int> action)
        {
            _matchCount.OnValueChanged -= action;
        }

        public bool CanPlayCard(CardController cardController)
        {
            if (cardController.EnergyCost < _matchCount.Value)
            {
                return false;
            }

            IsLastCardMatch = IsCardMatching(cardController);
            return IsLastCardMatch;
        }

        private bool IsCardMatching(CardController cardController)
        {
            return CurrentSuit switch
            {
                Suit.White => true,
                Suit.Black => false,
                _ => cardController.Suit == CurrentSuit || cardController.Rank == CurrentRank
            };
        }

        private void Start()
        {
            OnStartCombat();
        }

        public void OnStartCombat()
        {
            CurrentSuit = Suit.White;
            CurrentRank = 0;
        }


        public void OnTurnEnd()
        {
            _matchCount.Value = 0;
        }


        private void UpdateCurrentSuitAndRank(Suit suit, int rank)
        {
            CurrentSuit = suit;
            CurrentRank = rank;
            OnMatchValuesChanged?.Invoke(CurrentSuit, CurrentRank);
        }
    }
}