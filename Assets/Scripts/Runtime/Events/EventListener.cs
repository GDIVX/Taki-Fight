using System;
using UnityEngine;
using Runtime;

namespace Runtime.Events
{
    public class EventListener<T>
    {
        private Action<T> _eventHandler;
        private bool _subscribedToEventBus = false;

        public EventListener(Action<T> eventHandler)
        {
            _eventHandler = eventHandler;

            if (GameManager.Instance != null && GameManager.Instance.EventBus != null)
            {
                // EventBus is ready – subscribe immediately.
                GameManager.Instance.EventBus.Subscribe(_eventHandler);
                _subscribedToEventBus = true;
            }
            else
            {
                // EventBus isn't available yet. Subscribe to GameManager's static event.
                GameManager.Instance.OnEventBusCreated += HandleEventBusCreated;
            }
        }

        private void HandleEventBusCreated()
        {
            // Unsubscribe from the static event so we don't get called multiple times.
            GameManager.Instance.OnEventBusCreated -= HandleEventBusCreated;

            if (GameManager.Instance != null && GameManager.Instance.EventBus != null)
            {
                GameManager.Instance.EventBus.Subscribe(_eventHandler);
                _subscribedToEventBus = true;
            }
        }

        public void Disable()
        {
            if (_subscribedToEventBus && GameManager.Instance != null && GameManager.Instance.EventBus != null)
            {
                GameManager.Instance.EventBus.Unsubscribe(_eventHandler);
            }
            else
            {
                // In case we haven't subscribed yet, make sure to unsubscribe from the static event.
                GameManager.Instance.OnEventBusCreated -= HandleEventBusCreated;
            }

            _eventHandler = null;
        }
    }
}