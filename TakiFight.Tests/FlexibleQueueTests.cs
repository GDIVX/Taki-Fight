using NUnit.Framework;
using Utilities;
using System;

namespace TakiFight.Tests;

public class FlexibleQueueTests
{
    [Test]
    public void EnqueueDequeue_ShouldFollowFIFO()
    {
        var queue = new FlexibleQueue<int>();
        queue.Enqueue(1);
        queue.Enqueue(2);
        Assert.That(queue.Dequeue(), Is.EqualTo(1));
        Assert.That(queue.Dequeue(), Is.EqualTo(2));
    }

    [Test]
    public void Peek_ShouldNotRemoveItem()
    {
        var queue = new FlexibleQueue<string>();
        queue.Enqueue("a");
        Assert.That(queue.Peek(), Is.EqualTo("a"));
        Assert.That(queue.Count, Is.EqualTo(1));
    }

    [Test]
    public void RemoveAt_ShouldRemoveCorrectItem()
    {
        var queue = new FlexibleQueue<int>();
        queue.Enqueue(1);
        queue.Enqueue(2);
        queue.Enqueue(3);
        queue.RemoveAt(1);
        Assert.That(queue.ElementAt(1), Is.EqualTo(3));
    }

    [Test]
    public void Dequeue_EmptyQueue_ShouldThrow()
    {
        var queue = new FlexibleQueue<int>();
        Assert.Throws<InvalidOperationException>(() => queue.Dequeue());
    }
}
