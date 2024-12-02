using System;
using Runtime.CardGameplay.Card;
using Sirenix.OdinInspector;
using UnityEngine;
using Utilities;

namespace Runtime.CardGameplay.Board
{
    public class BoardController : MonoBehaviour, IBoardController
    {
        [ShowInInspector, ReadOnly] private bool _isLastCardMatch;

        [ShowInInspector, ReadOnly] public Suit CurrentSuit { get; private set; }
        [ShowInInspector, ReadOnly] public int CurrentRank { get; private set; }


        [ShowInInspector, ReadOnly] private TrackedProperty<int> MatchCount { get; } = new();


        public event Action<Suit, int> OnMatchValuesChanged;

        private bool IsLastCardMatch
        {
            get => _isLastCardMatch;
            set => _isLastCardMatch = value;
        }


        public void UpdateCurrentSuitAndRank(ICardController cardController)
        {
            UpdateCurrentSuitAndRank(cardController.Suit, cardController.Rank);
            MatchCount.Value += 1;
        }

        public void RegisterToMatchCountChanged(Action<int> action)
        {
            MatchCount.OnValueChanged += action;
        }

        public void UnregisterToMatchCountChanged(Action<int> action)
        {
            MatchCount.OnValueChanged -= action;
        }

        public bool CanPlayCard(ICardController cardController)
        {
            if (cardController.EnergyCost > MatchCount.Value)
            {
                //TODO : Add feedback
                Debug.Log(
                    $"Can't play card due to energy cost. Card cost = {cardController.EnergyCost} || current energy = {MatchCount.Value}");
                return false;
            }

            IsLastCardMatch = IsCardMatching(cardController);
            return IsLastCardMatch;
        }

        private bool IsCardMatching(ICardController cardController)
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
            ResetSuitAndRank();
        }

        public void ResetSuitAndRank()
        {
            UpdateCurrentSuitAndRank(Suit.White, 0);
        }


        public void OnTurnEnd()
        {
            MatchCount.Value = 0;
            ResetSuitAndRank();
        }


        private void UpdateCurrentSuitAndRank(Suit suit, int rank)
        {
            CurrentSuit = suit;
            CurrentRank = rank;
            OnMatchValuesChanged?.Invoke(CurrentSuit, CurrentRank);
        }
    }
}