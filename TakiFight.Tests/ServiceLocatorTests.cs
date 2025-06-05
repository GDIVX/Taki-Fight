using NUnit.Framework;
using Utilities;

namespace TakiFight.Tests;

public class ServiceLocatorTests
{
    private class DummyService { }

    [Test]
    public void RegisterAndGet_ReturnsRegisteredInstance()
    {
        var service = new DummyService();
        ServiceLocator.Register(service);
        var resolved = ServiceLocator.Get<DummyService>();
        Assert.That(resolved, Is.SameAs(service));
        ServiceLocator.Unregister(service);
    }

    [Test]
    public void TryGet_ReturnsTrueWhenRegistered()
    {
        var service = new DummyService();
        ServiceLocator.Register(service);
        bool found = ServiceLocator.TryGet(out DummyService resolved);
        Assert.That(found, Is.True);
        Assert.That(resolved, Is.SameAs(service));
        ServiceLocator.Unregister(service);
    }

    [Test]
    public void Unregister_RemovesService()
    {
        var service = new DummyService();
        ServiceLocator.Register(service);
        ServiceLocator.Unregister(service);
        bool found = ServiceLocator.TryGet(out DummyService resolved);
        Assert.That(found, Is.False);
        Assert.That(resolved, Is.Null);
    }
}
