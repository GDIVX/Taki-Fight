using System;
using System.Collections.Generic;

namespace Runtime.Events
{
    public class EventBus
    {
        private readonly Dictionary<Type, List<Delegate>> _subscribers = new Dictionary<Type, List<Delegate>>();

        public void Subscribe<T>(Action<T> handler)
        {
            var type = typeof(T);
            if (!_subscribers.ContainsKey(type))
            {
                _subscribers[type] = new List<Delegate>();
            }

            _subscribers[type].Add(handler);
        }

        public void Unsubscribe<T>(Action<T> handler)
        {
            var type = typeof(T);
            if (_subscribers.TryGetValue(type, out var subscriber))
            {
                subscriber.Remove(handler);
            }
        }

        public void Publish<T>(T eventData)
        {
            var type = typeof(T);
            if (_subscribers.TryGetValue(type, out var subscriber))
            {
                // Copy the list to avoid issues if handlers unsubscribe during iteration
                var handlers = new List<Delegate>(subscriber);
                foreach (var handler in handlers)
                {
                    ((Action<T>)handler)?.Invoke(eventData);
                }
            }
        }
    }
}