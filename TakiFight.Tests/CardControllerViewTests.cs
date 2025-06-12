using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace TakiFight.Tests
{
    public class CardControllerViewTests
    {
        private class TestStrategy
        {
            public int Potency { get; set; }
            public string GetDescription()
            {
                return $"Damage {Potency}";
            }
        }

        private struct PlayStrategyData
        {
            public TestStrategy PlayStrategy;
            public int Potency;
        }

        private class CardData
        {
            public int Cost { get; set; }
            public List<PlayStrategyData> PlayStrategies { get; } = new();
            public bool IsConsumed { get; set; }
            public string Title { get; set; }
        }

        private class CardInstance
        {
            public CardData Data { get; }
            public int Cost { get; set; }

            public CardInstance(CardData data)
            {
                Data = data;
                Cost = data.Cost;
            }
        }

        private class TestCardView
        {
            public string Description { get; private set; } = string.Empty;
            public int CostShown { get; private set; }
            private CardData _cardData;
            private CardController _controller;

            public void Draw(CardController controller)
            {
                _cardData = controller.Instance.Data;
                _controller = controller;
                SetCost(controller.Instance.Cost);
                UpdateDescription();
            }

            public void SetCost(int cost)
            {
                CostShown = cost;
            }

            public void UpdateDescription()
            {
                var builder = new DescriptionBuilder();
                Description = _controller != null ? builder.Build(_controller) : builder.Build(_cardData);
            }
        }

        private class CardController : UnityEngine.MonoBehaviour
        {
            public CardInstance Instance { get; private set; }
            public TestCardView View { get; set; }
            private readonly List<PlayStrategyData> _playStrategies = new();
            public IReadOnlyList<PlayStrategyData> PlayStrategies => _playStrategies;
            public CardData Data => Instance.Data;

            public void Init(CardInstance instance)
            {
                Instance = instance;
                _playStrategies.AddRange(instance.Data.PlayStrategies);
            }

            public int Cost
            {
                get => Instance.Cost;
                set
                {
                    Instance.Cost = value;
                    View?.SetCost(value);
                    View?.UpdateDescription();
                }
            }

            public void SetPotency(int index, int value)
            {
                var ps = _playStrategies[index];
                ps.PlayStrategy.Potency = value;
                _playStrategies[index] = ps;
                View?.UpdateDescription();
            }
        }

        private class DescriptionBuilder
        {
            public string Build(CardData data)
            {
                var sb = new StringBuilder();
                bool first = true;
                foreach (var ps in data.PlayStrategies)
                {
                    if (!first) sb.Append('\n');
                    sb.Append(ps.PlayStrategy.GetDescription());
                    first = false;
                }
                return sb.ToString();
            }

            public string Build(CardController controller)
            {
                var sb = new StringBuilder();
                bool first = true;
                foreach (var ps in controller.PlayStrategies)
                {
                    if (!first) sb.Append('\n');
                    sb.Append(ps.PlayStrategy.GetDescription());
                    first = false;
                }
                return sb.ToString();
            }
        }

        [Test]
        public void CostChange_UpdatesView()
        {
            var strat = new TestStrategy { Potency = 1 };
            var data = new CardData { Cost = 1, Title = "T" };
            data.PlayStrategies.Add(new PlayStrategyData { PlayStrategy = strat, Potency = 1 });
            var instance = new CardInstance(data);
            var controller = new CardController();
            controller.Init(instance);
            var view = new TestCardView();
            controller.View = view;
            view.Draw(controller);

            Assert.That(view.CostShown, Is.EqualTo(1));
            controller.Cost = 3;
            Assert.That(view.CostShown, Is.EqualTo(3));
        }

        [Test]
        public void PotencyChange_UpdatesDescription()
        {
            var strat = new TestStrategy { Potency = 1 };
            var data = new CardData { Cost = 1, Title = "T" };
            data.PlayStrategies.Add(new PlayStrategyData { PlayStrategy = strat, Potency = 1 });
            var instance = new CardInstance(data);
            var controller = new CardController();
            controller.Init(instance);
            var view = new TestCardView();
            controller.View = view;
            view.Draw(controller);

            Assert.That(view.Description, Is.EqualTo("Damage 1"));
            controller.SetPotency(0, 5);
            Assert.That(view.Description, Is.EqualTo("Damage 5"));
        }
    }
}
