using NUnit.Framework;
using Utilities;

namespace Tests.EditorMode
{
    public class StatTests
    {
        [Test]
        public void SettingBaseValue_UpdatesValue()
        {
            var stat = new Stat(5);
            stat.BaseValue = 10;
            Assert.That(stat.Value, Is.EqualTo(10));
        }

        [Test]
        public void SetModifier_UpdatesValueWithModifiers()
        {
            var stat = new Stat(10);
            object modA = new object();
            stat.SetModifier(modA, 5);
            Assert.That(stat.Value, Is.EqualTo(15));
        }

        [Test]
        public void RemoveModifier_RecalculatesValue()
        {
            var stat = new Stat(10);
            object modA = new object();
            object modB = new object();
            stat.SetModifier(modA, 5);
            stat.SetModifier(modB, -3);
            Assert.That(stat.Value, Is.EqualTo(12));
            stat.RemoveModifier(modA);
            Assert.That(stat.Value, Is.EqualTo(7));
        }

        [Test]
        public void ForceRecalculate_RaisesOnValueChanged()
        {
            var stat = new Stat(3);
            int notified = 0;
            stat.OnValueChanged += v => notified = v;
            stat.ForceRecalculate();
            Assert.That(notified, Is.EqualTo(3));
        }
    }
}

