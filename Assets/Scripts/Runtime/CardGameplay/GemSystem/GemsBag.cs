using System;
using System.Collections.Generic;
using System.Linq;
using Runtime.CardGameplay.Card;
using Sirenix.OdinInspector;
using UnityEngine;
using Utilities;

namespace Runtime.CardGameplay.GemSystem
{
    public class GemsBag : MonoBehaviour
    {
        [ShowInInspector, ReadOnly] private List<Gem> _content = new();
        [ShowInInspector, ReadOnly] private List<Gem> _gemsAvailableList = new();
        private Dictionary<GemType, int> _gemsAvailableCount;

        [SerializeField] private int _defaultDrawAmount = 3;
        [SerializeField, Required] private BagView _view;

        public event Action<List<Gem>> OnContentModifiedEvent;
        public event Action<List<Gem>> OnAvailableModified;

        public int DefaultDrawAmount
        {
            get => _defaultDrawAmount;
            set => _defaultDrawAmount = value;
        }

        public int Count => _content.Count;

        #region Initialization

        public void Initialize(GemGroup gems)
        {
            // Reset dictionary and both lists
            _gemsAvailableCount = Enum.GetValues(typeof(GemType))
                .Cast<GemType>()
                .ToDictionary(type => type, _ => 0);

            _content.Clear();
            _gemsAvailableList.Clear();

            // Add everything without event spam
            Add(GemType.Pearl, gems.Pearls, invokeEvents: false);
            Add(GemType.Quartz, gems.Quartz, invokeEvents: false);
            Add(GemType.Brimstone, gems.Brimstone, invokeEvents: false);

            // Fire events once
            OnContentModifiedEvent?.Invoke(_content);
            OnAvailableModified?.Invoke(_gemsAvailableList);

            // Initialize the view (UI or visual representation)
            _view.Initialize(this);
        }

        #endregion

        #region Adding Gems

        /// <summary>
        /// Adds a number of gems of a certain type to the bag (content).
        /// </summary>
        /// <param name="gemType">Type of gem to add.</param>
        /// <param name="count">How many to add.</param>
        /// <param name="invokeEvents">Set to false if you want to batch multiple adds without spamming events.</param>
        public void Add(GemType gemType, int count, bool invokeEvents = true)
        {
            for (int i = 0; i < count; i++)
            {
                var gem = new Gem { Type = gemType };
                _content.Add(gem);
            }

            if (invokeEvents)
            {
                OnContentModifiedEvent?.Invoke(_content);
                OnAvailableModified?.Invoke(_gemsAvailableList);
            }
        }

        #endregion

        #region Helpers / Checks

        public int CountAvailable(GemType gemType) => _gemsAvailableCount[gemType];

        public bool Has(GemType gemType, int count = 1)
        {
            return _gemsAvailableCount.TryGetValue(gemType, out int available) && available >= count;
        }

        public bool Has(int pearls, int quartz, int brimstone)
        {
            return Has(GemType.Pearl, pearls)
                   && Has(GemType.Quartz, quartz)
                   && Has(GemType.Brimstone, brimstone);
        }

        #endregion

        #region Drawing Gems

        /// <summary>
        /// Draws a number of random gems from the bag (content) into the "available" pool.
        /// </summary>
        public void Draw(int amount = 0)
        {
            amount = (amount <= 0) ? _defaultDrawAmount : amount;
            var drawnGems = new List<Gem>();

            for (int i = 0; i < amount; i++)
            {
                if (_content.Count == 0) break;
                var gem = _content.SelectRandom();
                _content.Remove(gem);
                _gemsAvailableList.Add(gem);
                _gemsAvailableCount[gem.Type]++;
                drawnGems.Add(gem);
            }

            // Single event trigger
            OnContentModifiedEvent?.Invoke(_content);
            OnAvailableModified?.Invoke(_gemsAvailableList);
        }

        /// <summary>
        /// Draws a specific number of gems of one type from the bag into the available pool.
        /// </summary>
        public void Draw(GemType type, int amount)
        {
            var drawnGems = new List<Gem>();

            for (int i = 0; i < amount; i++)
            {
                int index = _content.FindIndex(g => g.Type == type);
                if (index == -1) break;
                var gem = _content[index];
                _content.RemoveAt(index);
                _gemsAvailableList.Add(gem);
                _gemsAvailableCount[gem.Type]++;
                drawnGems.Add(gem);
            }

            OnContentModifiedEvent?.Invoke(_content);
            OnAvailableModified?.Invoke(_gemsAvailableList);
        }

        #endregion

        #region Reroll

        /// <summary>
        /// Takes all currently available gems, puts them back, then draws new ones.
        /// </summary>
        public void Reroll()
        {
            var oldAvailable = new List<Gem>(_gemsAvailableList);

            // Remove everything from the available pool without firing events yet
            foreach (var gem in oldAvailable)
            {
                _gemsAvailableList.Remove(gem);
                _gemsAvailableCount[gem.Type] = Mathf.Max(0, _gemsAvailableCount[gem.Type] - 1);
            }

            // Draw new gems
            Draw(_defaultDrawAmount);

            // Return old gems to the bag
            _content.AddRange(oldAvailable);

            // Finally, fire events once
            OnContentModifiedEvent?.Invoke(_content);
            OnAvailableModified?.Invoke(_gemsAvailableList);
        }

        #endregion

        #region Returning Gems

        /// <summary>
        /// Returns a single gem to the bag from the available pool.
        /// </summary>
        public void ReturnToBag(Gem gem)
        {
            if (_gemsAvailableList.Remove(gem))
            {
                _gemsAvailableCount[gem.Type] = Mathf.Max(0, _gemsAvailableCount[gem.Type] - 1);
            }

            _content.Add(gem);

            OnContentModifiedEvent?.Invoke(_content);
            OnAvailableModified?.Invoke(_gemsAvailableList);
        }

        /// <summary>
        /// Returns multiple gems of a certain type to the bag from the available pool.
        /// </summary>
        public void ReturnToBag(GemType type, int count)
        {
            for (int i = 0; i < count; i++)
            {
                int index = _gemsAvailableList.FindIndex(g => g.Type == type);
                if (index == -1) break;

                var gem = _gemsAvailableList[index];
                _gemsAvailableList.RemoveAt(index);
                _gemsAvailableCount[type] = Mathf.Max(0, _gemsAvailableCount[type] - 1);
                _content.Add(gem);
            }

            OnContentModifiedEvent?.Invoke(_content);
            OnAvailableModified?.Invoke(_gemsAvailableList);
        }

        #endregion

        #region Destroying Gems

        /// <summary>
        /// Destroys multiple gems of a specific type from the available pool.
        /// </summary>
        public void DestroyGems(GemType type, int amount)
        {
            int destroyed = 0;

            for (int i = 0; i < amount; i++)
            {
                int index = _gemsAvailableList.FindIndex(g => g.Type == type);
                if (index == -1) break;

                var gem = _gemsAvailableList[index];
                DestroyGem(gem, fireEvents: false);
                destroyed++;
            }

            // Fire events once if we destroyed any
            if (destroyed > 0)
            {
                OnContentModifiedEvent?.Invoke(_content);
                OnAvailableModified?.Invoke(_gemsAvailableList);
            }
        }

        /// <summary>
        /// Completely destroys a single gem instance (if it exists in either collection).
        /// </summary>
        private void DestroyGem(Gem gem, bool fireEvents = true)
        {
            gem.OnDestroy(); // handle the gem’s internal cleanup

            // Remove from available if present
            if (_gemsAvailableList.Remove(gem))
            {
                _gemsAvailableCount[gem.Type] = Mathf.Max(0, _gemsAvailableCount[gem.Type] - 1);
            }
            else
            {
                // Remove from content if present
                _content.Remove(gem);
            }


            // By default, destroy does not spam events; public methods do that
            if (fireEvents)
            {
                OnContentModifiedEvent?.Invoke(_content);
                OnAvailableModified?.Invoke(_gemsAvailableList);
            }
        }

        #endregion
    }
}