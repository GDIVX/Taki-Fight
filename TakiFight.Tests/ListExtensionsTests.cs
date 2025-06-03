using NUnit.Framework;
using Utilities;
using System.Collections.Generic;

namespace TakiFight.Tests;

public class ListExtensionsTests
{
    [SetUp]
    public void Setup()
    {
        UnityEngine.Random.InitState(0);
    }

    [Test]
    public void Shuffle_ShouldReorderList()
    {
        var list = new List<int> {1,2,3,4};
        list.Shuffle();
        CollectionAssert.AreEquivalent(new[]{1,2,3,4}, list);
        Assert.That(list, Is.Not.EqualTo(new List<int>{1,2,3,4}));
    }

    [Test]
    public void WeightedSelectRandom_ShouldReturnItemWithHighestWeight_WhenRandomIsHigh()
    {
        var weighted = new List<(string item, float weight)> {
            ("a", 0.1f),
            ("b", 0.9f)
        };
        UnityEngine.Random.InitState(int.MaxValue);
        string selected = weighted.ConvertAll(w => w.item).WeightedSelectRandom(i => weighted.Find(x => x.item==i).weight);
        Assert.That(selected, Is.EqualTo("b"));
    }
}
