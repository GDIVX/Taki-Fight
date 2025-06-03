using System;
using System.Text.RegularExpressions;
using Runtime.CardGameplay.Card.CardBehaviour;
using Sirenix.OdinInspector;

namespace Runtime.CardGameplay.Card
{
    [Serializable]
    public struct PlayStrategyData : IDescribable
    {
        [LabelText("Strategy")] public CardPlayStrategy PlayStrategy;

        [LabelText("Potency")] [MinValue(0)] public int Potency;


        public string GetDescription()
        {
            var description = PlayStrategy.GetDescription();
            // Replace the {Potency} token with the value of the Potency field
            var formattedDescription = description.Replace("{Potency}", Potency.ToString());

            // Use a regex to find all other tokens within curly brackets
            var tokenRegex = new Regex(@"\{([^}]+)\}");
            var matches = tokenRegex.Matches(formattedDescription);


            // Return the fully formatted description
            return formattedDescription;
        }
    }
}