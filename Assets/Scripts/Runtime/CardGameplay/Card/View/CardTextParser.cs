using TMPro;
using UnityEngine;
using System.Text.RegularExpressions;

namespace Runtime.CardGameplay.Card.View
{
    public class CardTextParser : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _textField;

        /// <summary>
        /// Replaces placeholders in rawText with dynamic values from the controller’s data.
        /// {cardType} -> CardType string
        /// {potency:X} -> Replaces with GetPotency(X)
        /// {attackValue:X} -> Replaces with GetPotency(X) + attackModifier
        /// {defenseValue:X} -> Replaces with GetPotency(X) + defenseModifier
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

            _textField.text = parsedText;
        }
    }
}