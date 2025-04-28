using Runtime.UI.Tooltip;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using Utilities;

namespace Runtime.CardGameplay.Card.View
{
    public class CardTextParser : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _textField;

        [Header("Keyword Formatting")] [SerializeField]
        private Color keywordColor = Color.yellow;

        [Header("Modified Value Formatting")] [SerializeField]
        private Color modifiedValueColor = Color.green;

        private static readonly Dictionary<string, Func<CardController, int, (string text, bool isModified)>>
            PlaceholderHandlers = new()
            {
                {
                    "potency", (controller, index) => (
                        controller.GetPotency(index).ToString(),
                        false // Base potency is never considered modified
                    )
                },
                {
                    "attack", (controller, index) =>
                    {
                        int baseValue = controller.GetPotency(index);
                        // int modifier = controller.Pawn?.AttackModifier.Value ?? 0;
                        return (
                            (baseValue).ToString(),
                            false
                        );
                    }
                },
                {
                    "defense", (controller, index) =>
                    {
                        int baseValue = controller.GetPotency(index);
                        // int modifier = controller.Pawn?.DefenseModifier.Value ?? 0;
                        return (
                            (baseValue).ToString(),
                            false
                        );
                    }
                },

                {
                    "heal", (controller, index) =>
                    {
                        int baseValue = controller.GetPotency(index);
                        return (
                            (baseValue).ToString(),
                            false
                        );
                    }
                }
            };

        private static readonly Regex PlaceholderPattern = new(
            @"\{(?<type>[^:}]+)(?::(?<index>\d+))?\}",
            RegexOptions.Compiled
        );

        /// <summary>
        /// Formats text with the specified color and bold styling
        /// </summary>
        private string FormatText(string text, Color color)
        {
            string colorHex = ColorUtility.ToHtmlStringRGB(color);
            return $"<b><color=#{colorHex}>{text}</color></b>";
        }

        /// <summary>
        /// Parses and updates the card's text description with dynamic values and icons.
        /// </summary>
        /// <param name="controller">The card controller containing game logic and data</param>
        /// <param name="rawText">The unparsed text containing placeholders</param>
        /// <exception cref="ArgumentException">Thrown when an invalid placeholder type is encountered</exception>
        public void DrawTextDescription(CardController controller, string rawText)
        {
            if (controller == null || string.IsNullOrEmpty(rawText))
            {
                _textField.text = rawText;
                return;
            }

            try
            {
                string parsedText = ParsePlaceholders(controller, rawText);
                parsedText = FormatKeywords(parsedText);
                _textField.text = parsedText;
            }
            catch (Exception e)
            {
                Debug.LogError($"Error parsing card text: {e.Message}");
                _textField.text = rawText; // Fallback to raw text on error
            }
        }

        /// <summary>
        /// Processes all placeholders in the text using registered handlers
        /// </summary>
        private string ParsePlaceholders(CardController controller, string text)
        {
            return PlaceholderPattern.Replace(text, match =>
            {
                string type = match.Groups["type"].Value.ToLowerInvariant();

                // Handle special case for cardType
                if (type == "cardtype")
                    return controller.CardType.ToString();

                // Handle indexed placeholders
                if (!match.Groups["index"].Success)
                    return match.Value;

                if (!PlaceholderHandlers.TryGetValue(type, out var handler))
                {
                    Debug.LogWarning($"Unknown placeholder type: {type}");
                    return match.Value;
                }

                int index = int.Parse(match.Groups["index"].Value);
                if (index < 0 || index >= controller.Data.PlayStrategies.Count)
                {
                    Debug.LogWarning($"Index out of range for {type}: {index}");
                    return match.Value;
                }

                var (value, isModified) = handler(controller, index);
                return isModified ? FormatText(value, modifiedValueColor) : value;
            });
        }

        /// <summary>
        /// Formats keywords from the KeywordDictionary with bold and color styling
        /// </summary>
        private string FormatKeywords(string text)
        {
            var keywordDictionary = ServiceLocator.Get<KeywordDictionary>();
            if (keywordDictionary?.Keywords == null || !keywordDictionary.Keywords.Any())
            {
                Debug.LogWarning("KeywordDictionary is empty or not assigned.");
                return text;
            }

            // Build a single regex pattern for all keywords
            var keywords = keywordDictionary.Keywords
                .Where(k => !string.IsNullOrEmpty(k))
                .Select(Regex.Escape)
                .OrderByDescending(k => k.Length); // Process longer keywords first

            string pattern = $@"\b(?:{string.Join("|", keywords)})\b";
            var keywordRegex = new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.Compiled);

            return keywordRegex.Replace(text, match =>
                match.Value.Contains("<b>") ? match.Value : FormatText(match.Value, keywordColor));
        }

        // Optional: Add method to preview colors in editor
        private void OnValidate()
        {
            // Ensure colors are not fully transparent
            keywordColor.a = 1f;
            modifiedValueColor.a = 1f;
        }
    }
}