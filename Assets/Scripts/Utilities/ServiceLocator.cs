using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

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
            if (Services.TryGetValue(typeof(T), out var instance) && instance is T typedInstance) return typedInstance;

            //if T is a unity object, try to find it in the scene and register it
            if (typeof(Object).IsAssignableFrom(typeof(T)))
            {
                if (Object.FindAnyObjectByType(typeof(T)) is T foundObject)
                {
                    Register(foundObject);
                    return foundObject;
                }

                Debug.LogWarning($"Could not find object of type {typeof(T).Name} in the scene.");
                return null;
            }

            //else, if it is a standalone class, create a new instance and register it
            try
            {
                var newInstance = Activator.CreateInstance<T>();
                Register(newInstance);
                return newInstance;
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to create instance of {typeof(T).Name}: {e.Message}");
                return null;
            }
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