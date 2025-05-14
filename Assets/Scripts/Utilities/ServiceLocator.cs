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

        // ReSharper disable Unity.PerformanceAnalysis
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
                //if it is a game object
                if (Object.FindAnyObjectByType(typeof(T)) is T foundObject)
                {
                    Register(foundObject); // Cache it for future use
                    return foundObject;
                }

                //if it is a scriptable object, try to find it in resources
                // Attempt to find the service if it's a ScriptableObject
                if (typeof(ScriptableObject).IsAssignableFrom(typeof(T)))
                {
                    // Try to load from Resources using the type name

                    if (Resources.Load(typeof(T).Name) is T resource) // If found in Resources
                    {
                        Register(resource); // Cache it for future use
                        return resource;
                    }

                    Debug.LogWarning($"ScriptableObject of type {typeof(T).Name} not found in Resources.");
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