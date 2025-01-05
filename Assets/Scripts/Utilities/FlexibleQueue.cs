using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;

namespace Utilities
{
    [Serializable]
    public class FlexibleQueue<T>
    {
        [ShowInInspector, ReadOnly] private readonly LinkedList<T> _list = new LinkedList<T>();

        /// <summary>
        /// Adds an item to the end of the queue.
        /// </summary>
        public void Enqueue(T item)
        {
            _list.AddLast(item);
        }

        /// <summary>
        /// Removes and returns the item at the front of the queue.
        /// </summary>
        /// <returns>The dequeued item.</returns>
        /// <exception cref="InvalidOperationException">Thrown when the queue is empty.</exception>
        public T Dequeue()
        {
            if (_list.Count == 0)
                throw new InvalidOperationException("Queue is empty.");

            var value = _list.First.Value;
            _list.RemoveFirst();
            return value;
        }

        /// <summary>
        /// Removes a specific item from the queue.
        /// </summary>
        /// <param name="item">The item to remove.</param>
        /// <returns>True if the item was found and removed; otherwise, false.</returns>
        public bool Remove(T item)
        {
            return _list.Remove(item);
        }

        /// <summary>
        /// Retrieves the item at the front of the queue without removing it.
        /// </summary>
        /// <returns>The item at the front of the queue.</returns>
        /// <exception cref="InvalidOperationException">Thrown when the queue is empty.</exception>
        public T Peek()
        {
            if (_list.Count == 0)
                throw new InvalidOperationException("Queue is empty.");

            return _list.First.Value;
        }

        /// <summary>
        /// Checks if the queue contains a specific item.
        /// </summary>
        /// <param name="item">The item to check for.</param>
        /// <returns>True if the item is found; otherwise, false.</returns>
        public bool Contains(T item)
        {
            return _list.Contains(item);
        }

        /// <summary>
        /// Gets the number of items in the queue.
        /// </summary>
        public int Count => _list.Count;

        /// <summary>
        /// Clears all items from the queue.
        /// </summary>
        public void Clear()
        {
            _list.Clear();
        }

        public List<T> ToList()
        {
            return _list.ToList();
        }
    }
}