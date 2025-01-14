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
        /// Replaces placeholders in rawText with dynamic values from the controller’s data.
        /// Keywords in the TooltipDictionary are underlined.
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

            // Regex pattern for {attack:X} and {defense:X} where X is the potency index
            int attackModifier = controller.Pawn != null ? controller.Pawn.AttackModifier.Value : 0;
            var attackValuePattern = @"\{attack:(\d+)\}";
            var defenseValuePattern = @"\{defense:(\d+)\}";

            // Handle attack values
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

            // Handle defense values
            int defenseModifier = controller.Pawn != null ? controller.Pawn.DefenseModifier.Value : 0;
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

            // Underline keywords from the TooltipDictionary
            parsedText = UnderlineKeywords(parsedText);

            _textField.text = parsedText;
        }

        /// <summary>
        /// Underlines keywords found in the TooltipDictionary within the given text.
        /// </summary>
        private string UnderlineKeywords(string text)
        {
            var _keywordDictionary = GameManager.Instance.KeywordDictionary;

            // Get keywords from the dictionary
            var keywords = _keywordDictionary.Keywords;
            if (keywords == null || !keywords.Any())
            {
                Debug.LogError("Dictionary dose not contain keys");
                return text;
            }

            // Iterate over each keyword
            foreach (var keyword in keywords)
            {
                if (string.IsNullOrEmpty(keyword))
                    continue;

                // Escape the keyword for Regex
                var escapedKeyword = Regex.Escape(keyword);

                // Regex pattern to match whole words (case-insensitive)
                var keywordPattern = $@"\b{escapedKeyword}\b";

                // Replace the matched keyword with underlined text
                text = Regex.Replace(text, keywordPattern, match =>
                {
                    // Avoid double-underlining or tag conflicts
                    if (match.Value.Contains("<u>") || match.Value.Contains("</u>"))
                        return match.Value;

                    return $"<u>{match.Value}</u>";
                }, RegexOptions.IgnoreCase);
            }

            return text;
        }
    }
}