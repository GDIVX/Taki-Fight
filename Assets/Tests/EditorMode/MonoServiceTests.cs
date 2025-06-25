using NUnit.Framework;
using UnityEngine;
using Utilities;

namespace Tests.EditorMode
{
    public class TestService : MonoService<TestService>
    {
    }

    public class MonoServiceTests
    {
        [Test]
        public void RegisterAndUnregisterTestService()
        {
            var go = new GameObject("TestService");
            var service = go.AddComponent<TestService>();

            Assert.That(ServiceLocator.Get<TestService>(), Is.SameAs(service));

            Object.DestroyImmediate(go);

            bool found = ServiceLocator.TryGet(out TestService _);
            Assert.That(found, Is.False);
        }
    }
}
