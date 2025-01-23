using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Runtime.CardGameplay.Card.CardBehaviour;
using Runtime.CardGameplay.Card.CardBehaviour.Feedback;
using Runtime.CardGameplay.Card.View;
using Runtime.CardGameplay.Deck;
using Runtime.CardGameplay.GemSystem;
using Runtime.Combat.Pawn;
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
        public CardType CardType { get; private set; }
        [ShowInInspector, ReadOnly] public TrackedProperty<bool> IsPlayable;

        [ShowInInspector, ReadOnly] private CardSelectStrategy _selectStrategy;
        [ShowInInspector, ReadOnly] private List<(CardPlayStrategy, int)> _playStrategies;
        [ShowInInspector, ReadOnly] private FeedbackStrategy _feedbackStrategy;

        public static event Action<CardController> OnCardPlayedEvent;

        public CardInstance Instance { get; private set; }
        public Transform Transform => gameObject.transform;
        public CardView View { get; private set; }


        public CardData Data { get; private set; }

        public HandController HandController { get; private set; }
        public GemsBag GemsBag { get; private set; }
        public PawnController Pawn { get; private set; }
        public GemGroup Group => Instance.Group;

        private CardFactory _cardFactory;

        [Button]
        public void Init(CardData data, CardDependencies dependencies)
        {
            if (data == null)
            {
                Debug.LogError("CardData cannot be null during initialization.");
                return;
            }

            Instance = new CardInstance(data)
            {
                Controller = this
            };

            HandController = dependencies.HandController;
            GemsBag = dependencies.GemsBag;
            Pawn = dependencies.Pawn;
            _cardFactory = dependencies.CardFactory;

            CardType = data.CardType;

            _selectStrategy = data.SelectStrategy;
            _feedbackStrategy = data.FeedbackStrategy;
            _playStrategies = CreatePlayStrategyTupletList(data.PlayStrategies);


            View = GetComponent<CardView>();

            //Card is selectable only when it can be played
            IsPlayable = new TrackedProperty<bool>()
            {
                Value = true
            };
            OnCardPlayedEvent += _ => UpdateAffordability();
            GemsBag.OnModifiedEvent += UpdateAffordability;
            Data = data;
        }

        private void UpdateAffordability()
        {
            IsPlayable.Value = CanAfford();
            View.UpdateDescription();
        }

        private List<(CardPlayStrategy, int)> CreatePlayStrategyTupletList(List<PlayStrategyData> dataPlayStrategies)
        {
            List<(CardPlayStrategy, int)> tuples = new();
            dataPlayStrategies.ForEach(x => tuples.Add((x.PlayStrategy, x.Potency)));
            return tuples;
        }

        public int GetPotency(int index)
        {
            return _playStrategies[index].Item2;
        }

        public void SetPotency(int index, int newValue)
        {
            (CardPlayStrategy, int) tuple = _playStrategies[index];
            _playStrategies.Remove(tuple);
            tuple.Item2 = newValue;
            _playStrategies.Insert(index, tuple);
        }

        public void Init(CardInstance cardInstance, CardDependencies dependencies)
        {
            if (cardInstance == null)
            {
                Debug.LogError("CardInstance cannot be null during initialization.");
                return;
            }

            Init(cardInstance.Data, dependencies);
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
            if (!CanAfford())
            {
                GameManager.Instance.BannerViewManager.WriteMessage(1, "Can't Afford to Play This Card", Color.red);
                this.Timer(1f, () => GameManager.Instance.BannerViewManager.Clear());
                return;
            }

            if (_feedbackStrategy)
            {
                Debug.Log("Starting feedback");
                _feedbackStrategy.Animate(Pawn, RunPlayLogic);
            }
            else
            {
                RunPlayLogic();
            }
        }

        [Button]
        private void RunPlayLogic()
        {
            foreach (var tuple in _playStrategies)
            {
                tuple.Item1.Play(Pawn, tuple.Item2);
            }

            HandleGemCost();

            if (Data.DestroyCardAfterUse)
            {
                HandController.BurnCard(this);
            }
            else
            {
                HandController.DiscardCard(this);
            }

            OnCardPlayedEvent?.Invoke(this);
        }

        private void HandleGemCost()
        {
            GemsBag.Remove(GemType.Pearl, Group.Pearls);
            GemsBag.Remove(GemType.Quartz, Group.Quartz);
            GemsBag.Remove(GemType.Brimstone, Group.Brimstone);
        }

        private bool CanAfford()
        {
            return GemsBag.Has(Group.Pearls, Group.Quartz, Group.Brimstone);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData == null)
            {
                Debug.LogWarning("OnPointerClick called with null eventData.");
                return;
            }

            switch (eventData.button)
            {
                case PointerEventData.InputButton.Left:
                    Select();
                    break;
            }
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
            UpdateAffordability();
        }


        private void TryToPlay()
        {
            if (!IsPlayable.Value)
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

    [Serializable]
    public struct GemGroup
    {
        public int Pearls;
        public int Quartz;
        public int Brimstone;
    }
}