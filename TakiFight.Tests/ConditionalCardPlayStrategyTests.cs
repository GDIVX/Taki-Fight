using System;
using System.Collections.Generic;

namespace TakiFight.Tests
{
    public class ConditionalCardPlayStrategyTests
    {
        private class CardPlayResult
        {
            public bool IsResolved { get; }
            public string EventType { get; }

            public CardPlayResult(bool resolved, string eventType = null)
            {
                IsResolved = resolved;
                EventType = eventType;
            }
        }

        private abstract class CardPlayStrategy
        {
            public abstract void Play(Action<CardPlayResult> onComplete);
        }

        private class ConditionalCardPlayStrategy : CardPlayStrategy
        {
            public CardPlayStrategy MainStrategy { get; set; } = null!;
            public CardPlayStrategy FollowUpStrategy { get; set; } = null!;
            public string ConditionEvent { get; set; } = string.Empty;

            public override void Play(Action<CardPlayResult> onComplete)
            {
                MainStrategy.Play(result =>
                {
                    if (result.IsResolved && result.EventType == ConditionEvent && FollowUpStrategy != null)
                    {
                        FollowUpStrategy.Play(_ => onComplete(result));
                    }
                    else
                    {
                        onComplete(result);
                    }
                });
            }
        }

        private class TestPrimaryStrategy : CardPlayStrategy
        {
            public override void Play(Action<CardPlayResult> onComplete)
            {
                onComplete(new CardPlayResult(true, "HitPawn"));
            }
        }

        private class TestSecondaryStrategy : CardPlayStrategy
        {
            public bool Executed { get; private set; }

            public override void Play(Action<CardPlayResult> onComplete)
            {
                Executed = true;
                onComplete(new CardPlayResult(true));
            }
        }

        private class TestCardController
        {
            public List<CardPlayStrategy> Strategies { get; } = new();

            public void RunPlayLogic()
            {
                foreach (var strategy in Strategies)
                {
                    strategy.Play(_ => { });
                }
            }
        }

        [Test]
        public void ConditionalStrategy_ExecutesFollowUp_WhenConditionMet()
        {
            var primary = new TestPrimaryStrategy();
            var secondary = new TestSecondaryStrategy();
            var conditional = new ConditionalCardPlayStrategy
            {
                MainStrategy = primary,
                FollowUpStrategy = secondary,
                ConditionEvent = "HitPawn"
            };

            var controller = new TestCardController();
            controller.Strategies.Add(conditional);

            controller.RunPlayLogic();

            Assert.That(secondary.Executed, Is.True);
        }
    }
}

