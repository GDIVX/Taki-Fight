using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace TakiFight.Tests;

public class SummonCardReturnTests
{
    private class CardData
    {
        public int Cost { get; set; }
    }

    private class Hand
    {
        public List<CardData> Cards { get; } = new();

        public void AddCard(CardData card)
        {
            Cards.Add(card);
        }
    }

    private class Pawn
    {
        public event Action OnKilled;

        public void Kill()
        {
            OnKilled?.Invoke();
        }
    }

    private class SummonUnitPlay
    {
        private readonly Hand hand;
        private readonly CardData card;
        private readonly bool returnCard;

        public SummonUnitPlay(Hand hand, CardData card, bool returnCard)
        {
            this.hand = hand;
            this.card = card;
            this.returnCard = returnCard;
        }

        public Pawn Summon()
        {
            var pawn = new Pawn();
            if (returnCard)
            {
                pawn.OnKilled += () =>
                {
                    var returned = new CardData { Cost = card.Cost + 1 };
                    hand.AddCard(returned);
                };
            }

            return pawn;
        }
    }

    [Test]
    public void KillingSummonedPawn_ReturnsCardWithIncreasedCost()
    {
        var hand = new Hand();
        var card = new CardData { Cost = 1 };
        var summon = new SummonUnitPlay(hand, card, true);

        var pawn = summon.Summon();
        pawn.Kill();

        Assert.That(hand.Cards.Count, Is.EqualTo(1));
        Assert.That(hand.Cards[0].Cost, Is.EqualTo(2));
    }

    [Test]
    public void KillingCardlessPawn_DoesNotChangeHand()
    {
        var hand = new Hand();
        var card = new CardData { Cost = 2 };
        var summon = new SummonUnitPlay(hand, card, false);

        var pawn = summon.Summon();
        pawn.Kill();

        Assert.That(hand.Cards.Count, Is.EqualTo(0));
    }
}
