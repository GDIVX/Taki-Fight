using System.Collections.Generic;
using System.Linq;
using Runtime.CardGameplay.Card.CardBehaviour;
using Runtime.CardGameplay.Card.CardBehaviour.Feedback;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace Runtime.CardGameplay.Card
{
    [CreateAssetMenu(fileName = "CardData", menuName = "Card/Data", order = 0)]
    public class CardData : ScriptableObject
    {
        // UI ──────────────────────────────────────────────────────────

        [HorizontalGroup("General", 60)]
        [PreviewField(60, ObjectFieldAlignment.Left)]
        [HideLabel]
        [SerializeField]
        [ValidateInput("IsImageEmpty", "Image must not be empty.")]
        private Sprite _image;

        [VerticalGroup("General/Info")]
        [LabelWidth(80)]
        [SerializeField]
        [ValidateInput("StringIsValid", "Title must not be empty.")]
        private string _title;

        [VerticalGroup("General/Info")]
        [TextArea(2, 4)]
        [SerializeField]
        [ValidateInput("StringIsValid", "Description must not be empty.")]
        private string _description;

        // Economy ────────────────────────────────────────────────────
        [VerticalGroup("General/Info")] [SerializeField] [MinValue(0)]
        private int _cost;

        // Meta ───────────────────────────────────────────────────────
        [HorizontalGroup("General/Info/Meta", 200)]
        [LabelText("Type")]
        [SerializeField]
        [ValidateInput("ValidateCardType", "$_typeValidationMessage")]
        private CardType _cardType;

        [HorizontalGroup("General/Info/Meta", 500, LabelWidth = 120)] [SerializeField] [EnumButtons]
        private Rarity _rarity;


        // Gameplay ───────────────────────────────────────────────────
        [TabGroup("Gameplay")] [ListDrawerSettings(ShowFoldout = true)] [SerializeField]
        private List<PlayStrategyData> _playStrategies;

        [TabGroup("Gameplay")] [LabelText("Consume After Use?")] [SerializeField]
        private bool _destroyCardAfterUse;


        // Feedback ───────────────────────────────────────────────────
        [TabGroup("Feedback")] [LabelText("VFX / SFX Strategy")] [SerializeField]
        private FeedbackStrategy _feedbackStrategy;

        [TabGroup("Danger Zone")]
        [GUIColor(1, 0f, 0)]
        [Button(ButtonSizes.Medium)]
        private void DeleteCard()
        {
            if (EditorUtility.DisplayDialog("Confirm Delete",
                    "Are you sure you want to delete this card?", "Yes", "No"))
                DestroyImmediate(this, true);
        }

        #region Properties

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

        #endregion

// Validation Methods

        #region Validation

        private string _typeValidationMessage;

        private bool StringIsValid(string str)
        {
            return !string.IsNullOrWhiteSpace(str);
        }

        private bool IsImageEmpty(Sprite sprite)
        {
            return sprite;
        }

        private bool ValidateCardType(CardType cardType)
        {
            //if we have a summon unit play strategy, this card must be familiar or totem 
            if (!_playStrategies.Any(strategy => strategy.PlayStrategy is SummonUnitPlay)
                || cardType is CardType.Familiar or CardType.Totem) return true;
            _typeValidationMessage = "Summon Unit Play Strategy requires a familiar or totem card.";
            return false;
        }

        #endregion
    }
}