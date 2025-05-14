using System.Collections.Generic;
using UnityEngine;

namespace Runtime.UI.OnScreenMessages
{
    public class MessageViewContainer : MonoBehaviour
    {
        private const int DefaultPreSpawnCount = 5;

        [SerializeField] [Tooltip("Message types this container handles")]
        private List<MessageType> _supportedTypes;

        [SerializeField] [Tooltip("Parent transform for layout")]
        private Transform _messageParentTransform;

        [SerializeField] [Tooltip("Prefab for spawning messages")]
        private MessageView _messagePrefab;

        private readonly List<MessageView> _messagePool = new();

        public void Initialize()
        {
            PreSpawnMessageViews(DefaultPreSpawnCount);
        }

        private void PreSpawnMessageViews(int count)
        {
            for (var i = 0; i < count; i++)
            {
                var messageView = Instantiate(_messagePrefab, _messageParentTransform);
                messageView.gameObject.SetActive(false);
                _messagePool.Add(messageView);
            }
        }

        public bool SupportsType(MessageType type)
        {
            return _supportedTypes.Contains(type);
        }

        public void ShowMessage(string message, float duration, Color? color)
        {
            var messageView = GetOrCreateMessageView();
            messageView.SetMessage(message, color ?? Color.white, duration);
        }

        private MessageView GetOrCreateMessageView()
        {
            var availableView = _messagePool.Find(view => !view.IsActive());
            if (availableView) return availableView;

            var newView = Instantiate(_messagePrefab, _messageParentTransform);
            _messagePool.Add(newView);
            return newView;
        }
    }
}