using System.Linq;
using TMPro;
using UnityEngine;
using System.Text.RegularExpressions;
using Runtime.UI.Tooltip;

namespace Runtime.CardGameplay.Card.View
{
    public class CardTextParser : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _textField;

        /// <summary>
        /// Parses and updates the card's text description with dynamic values and icons.
        /// </summary>
        public void DrawTextDescription(CardController controller, string rawText)
        {
            if (controller == null || string.IsNullOrEmpty(rawText))
            {
                _textField.text = rawText;
                return;
            }

            var parsedText = rawText;

            // Replace {cardType}
            parsedText = parsedText.Replace("{cardType}", controller.CardType.ToString());

            // Replace {potency:X}
            for (int i = 0; i < controller.Data.PlayStrategies.Count; i++)
            {
                string potencyPlaceholder = $"{{potency:{i}}}";
                if (parsedText.Contains(potencyPlaceholder))
                {
                    var potency = controller.GetPotency(i);
                    parsedText = parsedText.Replace(potencyPlaceholder, potency.ToString());
                }
            }

            // Handle {attack:X} and {defense:X} values with Regex
            int attackModifier = controller.Pawn != null ? controller.Pawn.AttackModifier.Value : 0;
            int defenseModifier = controller.Pawn != null ? controller.Pawn.DefenseModifier.Value : 0;

            var attackValuePattern = @"\{attack:(\d+)\}";
            var defenseValuePattern = @"\{defense:(\d+)\}";

            // Replace attack values
            parsedText = Regex.Replace(parsedText, attackValuePattern, match =>
            {
                if (int.TryParse(match.Groups[1].Value, out int index)
                    && index >= 0
                    && index < controller.Data.PlayStrategies.Count)
                {
                    int potency = controller.GetPotency(index);
                    int finalAttack = potency + attackModifier;
                    return finalAttack.ToString();
                }

                return match.Value;
            });

            // Replace defense values
            parsedText = Regex.Replace(parsedText, defenseValuePattern, match =>
            {
                if (int.TryParse(match.Groups[1].Value, out int index)
                    && index >= 0
                    && index < controller.Data.PlayStrategies.Count)
                {
                    int potency = controller.GetPotency(index);
                    int finalDefense = potency + defenseModifier;
                    return finalDefense.ToString();
                }

                return match.Value;
            });

            // Underline keywords
            parsedText = UnderlineKeywords(parsedText);

            // Assign the parsed text to the TMP field
            _textField.text = parsedText;
        }

        /// <summary>
        /// Underlines keywords from the TooltipDictionary within the text.
        /// </summary>
        private string UnderlineKeywords(string text)
        {
            var keywordDictionary = GameManager.Instance.KeywordDictionary;

            if (keywordDictionary == null || !keywordDictionary.Keywords.Any())
            {
                Debug.LogWarning("TooltipDictionary is empty or not assigned.");
                return text;
            }

            foreach (var keyword in keywordDictionary.Keywords)
            {
                if (string.IsNullOrEmpty(keyword)) continue;

                var escapedKeyword = Regex.Escape(keyword);
                var keywordPattern = $@"\b{escapedKeyword}\b";

                text = Regex.Replace(text, keywordPattern, match =>
                {
                    if (match.Value.Contains("<u>") || match.Value.Contains("</u>")) return match.Value;

                    return $"<u>{match.Value}</u>";
                }, RegexOptions.IgnoreCase);
            }

            return text;
        }
    }
}