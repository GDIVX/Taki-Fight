using NUnit.Framework;
using Utilities;

namespace TakiFight.Tests;

public class ObservableTests
{
    [Test]
    public void SettingValue_RaisesEvent()
    {
        var obs = new Observable<int>(1);
        int notified = 0;
        obs.OnValueChanged += v => notified = v;
        obs.Value = 5;
        Assert.That(notified, Is.EqualTo(5));
    }

    [Test]
    public void SettingSameValue_DoesNotRaiseEvent()
    {
        var obs = new Observable<int>(3);
        bool raised = false;
        obs.OnValueChanged += _ => raised = true;
        obs.Value = 3;
        Assert.That(raised, Is.False);
    }

    [Test]
    public void ForceNotify_RaisesEventWithCurrentValue()
    {
        var obs = new Observable<string>("hello");
        string notified = null;
        obs.OnValueChanged += v => notified = v;
        obs.ForceNotify();
        Assert.That(notified, Is.EqualTo("hello"));
    }
}
