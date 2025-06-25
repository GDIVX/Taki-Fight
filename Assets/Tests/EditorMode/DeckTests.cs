using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using Runtime.CardGameplay.Card;
using Runtime.CardGameplay.Deck;
using UnityEngine;

namespace Tests.EditorMode
{
    public class DeckTests
    {
        private CardInstance CreateCard(string title)
        {
            var data = ScriptableObject.CreateInstance<CardData>();
            data.Title = title;
            return new CardInstance(data);
        }

        private static Stack<CardInstance> GetPile(Deck deck, string fieldName)
        {
            var field = typeof(Deck).GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);
            return (Stack<CardInstance>)field.GetValue(deck);
        }

        private static Stack<CardInstance> GetDrawPile(Deck deck) => GetPile(deck, "_drawPile");
        private static Stack<CardInstance> GetDiscardPile(Deck deck) => GetPile(deck, "_discardPile");
        private static Stack<CardInstance> GetConsumePile(Deck deck) => GetPile(deck, "_consumePile");

        [Test]
        public void Draw_ReturnsCardAndUpdatesCount()
        {
            var deck = new Deck(new List<CardInstance> { CreateCard("A"), CreateCard("B") });
            int initial = GetDrawPile(deck).Count;
            bool result = deck.Draw(out var card);

            Assert.IsTrue(result);
            Assert.IsNotNull(card);
            Assert.That(GetDrawPile(deck).Count, Is.EqualTo(initial - 1));
        }

        [Test]
        public void Discard_Consume_AndMergeAndShuffle_MoveCardsBetweenPiles()
        {
            var cards = new List<CardInstance>
            {
                CreateCard("A"),
                CreateCard("B"),
                CreateCard("C")
            };
            var deck = new Deck(cards);

            deck.Draw(out var first);
            deck.Discard(first);
            Assert.IsTrue(deck.IsDiscarded(first));
            Assert.IsFalse(deck.IsInDrawPile(first));

            deck.Draw(out var second);
            deck.Consume(second);
            Assert.IsTrue(deck.IsConsumed(second));
            Assert.IsFalse(deck.IsInDrawPile(second));

            deck.MergeAndShuffle();
            Assert.IsFalse(deck.IsDiscarded(first));
            Assert.IsTrue(deck.IsInDrawPile(first));
            Assert.That(GetDiscardPile(deck).Count, Is.EqualTo(0));
            Assert.IsTrue(deck.IsConsumed(second));
            Assert.IsFalse(deck.IsInDrawPile(second));
        }

        [Test]
        public void TryToFindAndRemoveCard_RemovesSpecifiedCard()
        {
            var c1 = CreateCard("A");
            var c2 = CreateCard("B");
            var c3 = CreateCard("C");
            var deck = new Deck(new List<CardInstance> { c1, c2, c3 });
            int initial = GetDrawPile(deck).Count;

            bool found = deck.TryToFindAndRemoveCard(c2.Data, out var removed);

            Assert.IsTrue(found);
            Assert.AreSame(c2.Data, removed.Data);
            Assert.That(GetDrawPile(deck).Count, Is.EqualTo(initial - 1));
            Assert.IsFalse(deck.IsInDrawPile(removed));
        }
    }
}
