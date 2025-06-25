using NUnit.Framework;
using Runtime.CardGameplay.Card;
using Runtime.Combat.Pawn;

namespace Tests.EditorMode
{
    public class DescriptionBuilderTests
    {
        [Test]
        public void WithRelations_ReturnsExpectedText()
        {
            var playerBuilder = new DescriptionBuilder().WithRelations(PawnOwner.Player, true);
            var enemyBuilder = new DescriptionBuilder().WithRelations(PawnOwner.Enemy, false);

            Assert.That(playerBuilder.ToString(), Is.EqualTo("an <color=#00ff00>allied</color> Familiar"));
            Assert.That(enemyBuilder.ToString(), Is.EqualTo("<color=#ff0000>hostile</color> Familiars"));
        }

        [Test]
        public void WithSpace_AppendsSpace()
        {
            var result = new DescriptionBuilder()
                .Append("Hello")
                .WithSpace()
                .Append("World")
                .ToString();

            Assert.That(result, Is.EqualTo("Hello World"));
        }

        [Test]
        public void AppendBold_WrapsTextInBold()
        {
            var result = new DescriptionBuilder()
                .AppendBold("test")
                .ToString();

            Assert.That(result, Is.EqualTo("<b>test</b>"));
        }
    }
}
