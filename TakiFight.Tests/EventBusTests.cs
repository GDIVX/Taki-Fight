using NUnit.Framework;
using Runtime.Events;

namespace TakiFight.Tests;

public class EventBusTests
{
    [Test]
    public void SubscribeAndPublish_ShouldInvokeHandler()
    {
        var bus = new EventBus();
        int received = 0;
        bus.Subscribe<int>(v => received = v);
        bus.Publish(5);
        Assert.That(received, Is.EqualTo(5));
    }

    [Test]
    public void Unsubscribe_ShouldStopReceivingEvents()
    {
        var bus = new EventBus();
        int received = 0;
        void Handler(int v) => received = v;
        bus.Subscribe<int>(Handler);
        bus.Unsubscribe<int>(Handler);
        bus.Publish(10);
        Assert.That(received, Is.EqualTo(0));
    }
}
