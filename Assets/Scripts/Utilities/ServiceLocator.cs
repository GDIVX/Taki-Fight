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
            // Directly fetch the service if already registered
            if (Services.TryGetValue(typeof(T), out var instance))
            {
                // If it's a Unity Object, ensure that it's not destroyed
                if (instance is not Object unityObject) return (T)instance;
                if (unityObject != null) return (T)instance;

                // Object has been destroyed, clean up and attempt to find a replacement
                Services.Remove(typeof(T));

                // Attempt to find a replacement (dynamic lookup)
                if (Object.FindAnyObjectByType(typeof(T)) is T foundReplacement)
                {
                    Register(foundReplacement);
                    return foundReplacement;
                }

                Debug.LogWarning($"No replacement found for {typeof(T).Name} in the current scene.");
                return null;

                // If it's not a Unity object, return the instance safely
            }

            // If not found, and it's a Unity Object, try finding it in the scene (expensive but safe fallback)
            if (typeof(Object).IsAssignableFrom(typeof(T)))
            {
                var foundObject = Object.FindAnyObjectByType(typeof(T)) as T;
                if (foundObject != null)
                {
                    Register(foundObject); // Cache it for future use
                    return foundObject;
                }

                Debug.LogWarning($"Service of type {typeof(T).Name} not found in the current scene.");
                return null;
            }

            // If it's not a Unity Object, create it dynamically as a fallback
            try
            {
                var newInstance = Activator.CreateInstance<T>();
                Register(newInstance); // Cache it for future use
                return newInstance;
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to create an instance of {typeof(T).Name}: {e.Message}");
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

        public static void Unregister<T>(T service) where T : class
        {
            if (Services.TryGetValue(typeof(T), out var instance) && instance is T typedInstance)
                Services.Remove(typeof(T));
        }
    }
}