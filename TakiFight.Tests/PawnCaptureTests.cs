using NUnit.Framework;
using System.Collections.Generic;

namespace TakiFight.Tests;

public class PawnCaptureTests
{
    private class CardData
    {
        public PawnData Pawn { get; set; }
    }

    private class CardInstance
    {
        public CardData Data { get; }
        public CardInstance(CardData data) { Data = data; }
    }

    private class Hand
    {
        public List<CardInstance> Cards { get; } = new();
        public void AddCardFromInstant(CardInstance instance) => Cards.Add(instance);
    }

    private class PawnData
    {
        public CardData CreateRuntimeSummonCard(int cost)
        {
            return new CardData { Pawn = this };
        }
    }

    private class PawnController
    {
        private readonly Hand hand;
        public PawnData Data { get; }
        public bool Removed { get; private set; }
        public int Hp { get; set; }
        public PawnController(PawnData data, Hand hand)
        {
            Data = data;
            this.hand = hand;
            Hp = 1;
        }

        public bool Capture(int potency)
        {
            if (Hp > potency)
            {
                return false;
            }

            var cardData = Data.CreateRuntimeSummonCard(1);
            var instance = new CardInstance(cardData);
            hand.AddCardFromInstant(instance);
            Removed = true;
            return true;
        }
    }

    [Test]
    public void Capture_AddsCardToHand_WhenPotencySufficient()
    {
        var hand = new Hand();
        var data = new PawnData();
        var pawn = new PawnController(data, hand) { Hp = 1 };

        var result = pawn.Capture(2);

        Assert.That(result, Is.True);
        Assert.That(hand.Cards.Count, Is.EqualTo(1));
        Assert.That(hand.Cards[0].Data.Pawn, Is.SameAs(data));
        Assert.That(pawn.Removed, Is.True);
    }

    [Test]
    public void Capture_Fails_WhenPotencyTooLow()
    {
        var hand = new Hand();
        var data = new PawnData();
        var pawn = new PawnController(data, hand) { Hp = 5 };

        var result = pawn.Capture(2);

        Assert.That(result, Is.False);
        Assert.That(hand.Cards.Count, Is.EqualTo(0));
        Assert.That(pawn.Removed, Is.False);
    }
}
