using System.Collections.Generic;
using Runtime.CardGameplay.Card.CardBehaviour;
using Runtime.CardGameplay.Card.CardBehaviour.Feedback;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Runtime.CardGameplay.Card
{
    [CreateAssetMenu(fileName = "CardData", menuName = "Card/Data", order = 0)]
    public class CardData : ScriptableObject
    {
        // UI ──────────────────────────────────────────────────────────
        [TabGroup("Card", "UI")] [LabelWidth(80)] [SerializeField]
        private string _title;

        [TabGroup("Card", "UI")] [PreviewField(150, ObjectFieldAlignment.Center)] [HideLabel] [SerializeField]
        private Sprite _image;

        [TabGroup("Card", "UI")] [TextArea(2, 4)] [SerializeField]
        private string _description;

        // Meta ───────────────────────────────────────────────────────
        [TabGroup("Card", "Meta")] [LabelText("Type")] [SerializeField]
        private CardType _cardType;

        [TabGroup("Card", "Meta")] [LabelText("Rarity")] [SerializeField]
        private Rarity _rarity;

        // Gameplay ───────────────────────────────────────────────────
        [TabGroup("Card", "Gameplay")] [ListDrawerSettings(ShowFoldout = true)] [SerializeField]
        private List<PlayStrategyData> _playStrategies;

        [TabGroup("Card", "Gameplay")] [LabelText("Consume After Use?")] [SerializeField]
        private bool _destroyCardAfterUse;

        // Economy ────────────────────────────────────────────────────
        [TabGroup("Card", "Economy")] [LabelText("Cost")] [SerializeField]
        private int _cost;

        // Feedback ───────────────────────────────────────────────────
        [TabGroup("Card", "Feedback")] [LabelText("VFX / SFX Strategy")] [SerializeField]
        private FeedbackStrategy _feedbackStrategy;

        // Properties (with accessors intact)
        public string Title
        {
            get => _title;
            set => _title = value;
        }

        public string Description
        {
            get => _description;
            set => _description = value;
        }

        public Sprite Image
        {
            get => _image;
            set => _image = value;
        }

        public CardType CardType
        {
            get => _cardType;
            set => _cardType = value;
        }

        public Rarity Rarity
        {
            get => _rarity;
            set => _rarity = value;
        }

        public List<PlayStrategyData> PlayStrategies
        {
            get => _playStrategies;
            set => _playStrategies = value;
        }

        public bool DestroyCardAfterUse
        {
            get => _destroyCardAfterUse;
            set => _destroyCardAfterUse = value;
        }

        public int Cost
        {
            get => _cost;
            set => _cost = value;
        }

        public FeedbackStrategy FeedbackStrategy
        {
            get => _feedbackStrategy;
            set => _feedbackStrategy = value;
        }

        // Validation Methods
        [Button("Validate Card")]
        [InfoBox("This section highlights validation errors for debugging purposes.")]
        [ShowInInspector]
        [HideLabel]
        public string ValidationMessage => GetValidationMessage();

        private string GetValidationMessage()
        {
            var messages = new List<string>();

            // Hard validation
            if (string.IsNullOrEmpty(_title)) messages.Add("• Title is missing (Hard Validation).");
            if (_cost < 0) messages.Add("• Cost cannot be negative (Hard Validation).");
            if (_playStrategies == null || _playStrategies.Count == 0)
                messages.Add("• At least one play strategy is required (Hard Validation).");

            // Soft validation
            if (string.IsNullOrEmpty(_description)) messages.Add("• Description is missing (Soft Validation).");
            if (_image == null) messages.Add("• Image is missing (Soft Validation).");

            if (_playStrategies != null && _playStrategies.Exists(ps => ps.PlayStrategy is SummonUnitPlay))
                if (_cardType != CardType.Familiar)
                    messages.Add("• Card type must be 'Familiar' if it includes 'SummonUnitPlay' (Soft Validation).");

            return messages.Count > 0
                ? string.Join("\n", messages)
                : "Card is valid.";
        }
    }
}