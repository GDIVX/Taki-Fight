using System.Collections.Generic;
using NUnit.Framework;

namespace TakiFight.Tests
{
    public class CompoundTooltipBuilderTests
    {
        private class TooltipData
        {
            public string Header { get; set; }
            public string Description { get; set; }
        }

        private class CompoundTooltipData
        {
            public List<TooltipData> Tooltips { get; } = new();
            public void Add(TooltipData data) => Tooltips.Add(data);
        }

        private class Tile
        {
            public string Owner { get; set; }
        }

        private class Pawn
        {
            public string Name { get; set; }
        }

        private class DescriptionBuilder
        {
            public string AsPawn(Pawn pawn) => $"Pawn:{pawn.Name}";
        }

        private static CompoundTooltipData Build(Tile tile, Pawn pawn, IEnumerable<TooltipData> effects)
        {
            var compound = new CompoundTooltipData();
            compound.Add(new TooltipData { Header = $"Tile {tile.Owner}" });
            if (pawn != null)
            {
                var builder = new DescriptionBuilder();
                compound.Add(new TooltipData { Header = pawn.Name, Description = builder.AsPawn(pawn) });
            }
            if (effects != null)
            {
                foreach (var e in effects) compound.Add(e);
            }
            return compound;
        }

        [Test]
        public void Build_ComposesTilePawnAndEffectsInOrder()
        {
            var tile = new Tile { Owner = "Player" };
            var pawn = new Pawn { Name = "Goblin" };
            var effect1 = new TooltipData { Header = "Stun" };
            var effect2 = new TooltipData { Header = "Poison" };

            var result = Build(tile, pawn, new[] { effect1, effect2 });

            Assert.That(result.Tooltips.Count, Is.EqualTo(4));
            Assert.That(result.Tooltips[0].Header, Is.EqualTo("Tile Player"));
            Assert.That(result.Tooltips[1].Header, Is.EqualTo("Goblin"));
            Assert.That(result.Tooltips[2], Is.SameAs(effect1));
            Assert.That(result.Tooltips[3], Is.SameAs(effect2));
        }
    }
}
