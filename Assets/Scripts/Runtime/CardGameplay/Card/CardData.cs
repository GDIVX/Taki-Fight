using System;
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
// One tab bar: Card ▸ UI | Meta | Gameplay | Economy | Feedback
// ─────────────────────────────────────────────────────────────

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

        public Rarity Rarity => _rarity;

        public List<PlayStrategyData> PlayStrategies
        {
            get => _playStrategies;
            set => _playStrategies = value;
        }

        public bool DestroyCardAfterUse => _destroyCardAfterUse;

        public int Cost
        {
            get => _cost;
            set => _cost = value;
        }

        public FeedbackStrategy FeedbackStrategy => _feedbackStrategy;
    }

    [Serializable]
    public struct PlayStrategyData
    {
        [LabelText("Strategy")] public CardPlayStrategy PlayStrategy;

        [LabelText("Potency")] [MinValue(0)] public int Potency;
    }
}