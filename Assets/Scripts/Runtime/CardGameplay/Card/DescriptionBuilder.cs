using System.Collections.Generic;
using System.Linq;
using System.Text;
using Runtime.CardGameplay.Card.CardBehaviour;
using Runtime.CardGameplay.Card.View;
using Runtime.Combat.Pawn;

namespace Runtime.CardGameplay.Card
{
    public class DescriptionBuilder
    {
        private readonly StringBuilder _builder = new();
        private bool _isFirstLine = true;

        public string GetFormattedText()
        {
            return _builder.ToString().Trim();
        }

        /// <summary>
        ///     Adds a line describing triggered abilities.
        ///     Example: "Strike: +2 Attack."
        /// </summary>
        public DescriptionBuilder WithTriggeredAbilities(string triggerKeyword, List<IDescribable> abilities)
        {
            if (abilities == null || abilities.Count == 0) return this;

            AddNewLineIfNeeded();
            _builder.Append($"{Hyperlink(triggerKeyword)}: ");

            for (var i = 0; i < abilities.Count; i++)
            {
                _builder.Append(abilities[i].GetDescription());
                if (i < abilities.Count - 2)
                    _builder.Append(", ");
                else if (i == abilities.Count - 2) _builder.Append(" and ");
            }

            return this;
        }

        /// <summary>
        ///     Adds a single keyword with hyperlink formatting.
        ///     Example: "Consumed"
        /// </summary>
        public DescriptionBuilder WithKeyword(string keyword)
        {
            AddNewLineIfNeeded();
            _builder.Append($"{Hyperlink(keyword)}");
            return this;
        }

        public DescriptionBuilder WithKeyword(Keyword keyword)
        {
            AddNewLineIfNeeded();
            _builder.Append($"{Hyperlink(keyword.FormattedText)}");
            return this;
        }

        /// <summary>
        ///     Adds a line describing an ability.
        ///     Example: "+3 Defense Boost"
        /// </summary>
        public DescriptionBuilder WithLine(IDescribable describable)
        {
            if (describable == null) return this;

            AddNewLineIfNeeded();
            _builder.Append(describable.GetDescription());
            return this;
        }

        public DescriptionBuilder WithLine(string line)
        {
            if (line == null) return this;
            AddNewLineIfNeeded();
            _builder.Append(line);
            return this;
        }

        /// <summary>
        ///     Builds a formatted description for a spell card.
        /// </summary>
        public string AsSpell(CardData spellCard)
        {
            if (spellCard.IsConsumed) WithKeyword("Consume");

            foreach (var strategy in spellCard.PlayStrategies)
            {
                if (strategy.PlayStrategy is SummonUnitPlay summonStrategy) return AsSummon(summonStrategy.Pawn);

                WithLine(strategy.PlayStrategy);
            }

            return GetFormattedText();
        }

        /// <summary>
        ///     Builds a formatted description for a summon card.
        /// </summary>
        public string AsSummon(PawnData unit)
        {
            //stats

            //attack
            WithLine(unit.Attacks > 1
                ? $"Attack for {unit.Damage}X{unit.Attacks} {unit.DamageType.GetDescription()}"
                : $"Attack for {unit.Damage} {unit.DamageType.GetDescription()}");

            //defense
            if (unit.Defense > 0) WithLine($"Armor {unit.Defense}");

            //Health
            WithLine($"Health {unit.Health}");

            // On Summon
            WithTriggeredAbilities("Summon", GetDescribableAbilities(unit.OnSummonStrategies));

            // On Turn Start
            foreach (var strategy in unit.OnTurnStartStrategies) WithLine(strategy.Strategy);

            // On Attack
            WithTriggeredAbilities("Strike", GetDescribableAbilities(unit.OnAttackStrategies));

            // On Hit
            WithTriggeredAbilities("Hit", GetDescribableAbilities(unit.OnHitStrategies));

            // On Damaged
            WithTriggeredAbilities("Revenge", GetDescribableAbilities(unit.OnDamagedStrategies));

            // On Killed
            WithTriggeredAbilities("Death Rattle", GetDescribableAbilities(unit.OnKilledStrategies));

            WithTriggeredAbilities("Moved", GetDescribableAbilities(unit.OnMoveStrategies));

            return GetFormattedText();
        }

        /// <summary>
        ///     Utility for hyperlink formatting (e.g., tooltips).
        /// </summary>
        private static string Hyperlink(string text)
        {
            return $"<link=\"{text}\"><b>{text}</b></link>";
        }

        /// <summary>
        ///     Utility method to add a newline only when needed.
        /// </summary>
        private void AddNewLineIfNeeded()
        {
            if (_isFirstLine)
                _isFirstLine = false;
            else
                _builder.AppendLine();
        }

        /// <summary>
        ///     Extracts describable strategies from a list of pawn strategies.
        /// </summary>
        private List<IDescribable> GetDescribableAbilities(List<PawnStrategyData> strategyDataList)
        {
            return strategyDataList?
                .Select(data => data.Strategy as IDescribable)
                .Where(s => s != null)
                .ToList() ?? new List<IDescribable>();
        }

        public string Build(CardData cardData)
        {
            if (cardData.PlayStrategies[0].PlayStrategy is SummonUnitPlay summon) return AsSummon(summon.Pawn);

            return AsSpell(cardData);
        }

        public string Build(CardController controller)
        {
            if (controller.PlayStrategies.Count > 0 &&
                controller.PlayStrategies[0].PlayStrategy is SummonUnitPlay summon)
            {
                return AsSummon(summon.Pawn);
            }

            return AsSpell(controller);
        }

        private string AsSpell(CardController controller)
        {
            if (controller.Data.IsConsumed)
            {
                WithKeyword("Consume");
            }

            foreach (var strategy in controller.PlayStrategies)
            {
                if (strategy.PlayStrategy is SummonUnitPlay summonStrategy)
                {
                    return AsSummon(summonStrategy.Pawn);
                }

                WithLine(strategy.PlayStrategy);
            }

            return GetFormattedText();
        }
    }
}