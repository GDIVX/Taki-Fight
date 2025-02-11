using System;
using System.Collections.Generic;

namespace Utilities
{
    public static class ServiceLocator
    {
        private static readonly Dictionary<Type, object> Services = new();

        public static void Register<T>(T service) where T : class
        {
            var type = typeof(T);
            Services.TryAdd(type, service);
        }

        public static T Get<T>() where T : class
        {
            return Services[typeof(T)] as T;
        }

        public static bool TryGet<T>(out T service) where T : class
        {
            if (Services.TryGetValue(typeof(T), out var instance) && instance is T typedInstance)
            {
                service = typedInstance;
                return true;
            }

            service = null;
            return false;
        }
    }
}