using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Runtime.CardGameplay.Card.CardBehaviour;
using Runtime.CardGameplay.Card.View;
using Runtime.CardGameplay.Deck;
using Runtime.CardGameplay.GlyphsBoard;
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
    public class CardController : MonoBehaviour, IPointerClickHandler
    {
        public List<CardGlyph> Glyphs { get; private set; }
        public int Potency { get; private set; }
        public CardType CardType { get; private set; }
        [ShowInInspector, ReadOnly] public TrackedProperty<bool> IsPlayable;

        [ShowInInspector, ReadOnly] private CardSelectStrategy _selectStrategy;
        [ShowInInspector, ReadOnly] private CardPlayStrategy _playStrategy;
        [ShowInInspector, ReadOnly] private CardAffordabilityStrategy _affordabilityStrategy;
        [ShowInInspector, ReadOnly] private CardPostPlayStrategy _postPlayStrategy;
        [ShowInInspector, ReadOnly] private CardOnRankChangedEventHandler _rankChangedEventHandler;
        [ShowInInspector, ReadOnly] private CardOnSuitChangeEventHandler _suitChangeEventHandler;


        public static event Action<CardController> OnCardPlayed;

        public CardInstance Instance { get; private set; }
        public Transform Transform => gameObject.transform;
        public CardView View { get; private set; }


        public CardData Data { get; private set; }

        public HandController HandController { get; private set; }
        public GlyphBoardController GlyphBoardController { get; private set; }
        public PawnController Pawn { get; private set; }
        private CardFactory _cardFactory;

        [Button]
        public void Init(CardData data, List<CardGlyph> glyphs, CardDependencies dependencies)
        {
            if (data == null)
            {
                Debug.LogError("CardData cannot be null during initialization.");
                return;
            }

            HandController = dependencies.HandController;
            GlyphBoardController = dependencies.GlyphBoardController;
            Pawn = dependencies.Pawn;
            _cardFactory = dependencies.CardFactory;

            Glyphs = glyphs;
            Potency = data.Potency;
            CardType = data.CardType;

            _selectStrategy = data.SelectStrategy;
            _playStrategy = data.PlayStrategy;
            _affordabilityStrategy = data.AffordabilityStrategy;
            _postPlayStrategy = data.PostPlayStrategy;
            _rankChangedEventHandler = data?.RankChangedEventHandler;
            _suitChangeEventHandler = data?.SuitChangedEventHandler;


            Instance = new CardInstance(data, glyphs)
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

            Init(cardInstance.Data, cardInstance.Glyphs, dependencies);
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
            if (_playStrategy == null)
            {
                Debug.LogError("PlayStrategy is not set.");
                return;
            }

            //Play the card before discarding it or updating the current suit and rank.
            //In order to preserve the game state for any play strategy before doing changes 
            _playStrategy.Play(Pawn, Potency);
            _postPlayStrategy.PostPlay(this);
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
            IsPlayable.Value = CanPlayCard();
        }

        private bool CanPlayCard()
        {
            return _affordabilityStrategy.CanPlayCard(this);
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
    }
}