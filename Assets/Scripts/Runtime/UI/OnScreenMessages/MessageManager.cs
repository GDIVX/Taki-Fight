using System.Collections.Generic;
using UnityEngine;
using Utilities;

namespace Runtime.UI.OnScreenMessages
{
    public class MessageManager : MonoService<MessageManager>
    {
        [SerializeField] private List<MessageViewContainer> _containers;

        private void Awake()
        {
            // Initialize containers
            foreach (var container in _containers) container.Initialize();
        }

        public void ShowMessage(string message, MessageType type, float duration = 3f, Color? color = null)
        {
            // Locate the correct container based on message type
            var container = GetContainerForType(type);
            if (container)
                container.ShowMessage(message, duration, color);
            else
                Debug.LogWarning($"No container found for MessageType: {type}");
        }

        private MessageViewContainer GetContainerForType(MessageType type)
        {
            // Find the container matching the message type
            return _containers.Find(c => c.SupportsType(type));
        }
    }
}