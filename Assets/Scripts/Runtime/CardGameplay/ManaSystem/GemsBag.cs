using System;
using System.Collections.Generic;
using System.Linq;
using Runtime.CardGameplay.Card;
using Sirenix.OdinInspector;
using UnityEngine;
using Utilities;

namespace Runtime.CardGameplay.ManaSystem
{
    public class GemsBag : MonoBehaviour
    {
        /// <summary>
        /// The gems inside the bag
        /// </summary>
        [ShowInInspector, ReadOnly] private List<Gem> _content = new();

        /// <summary>
        /// The gems outside the bag
        /// </summary>
        [ShowInInspector, ReadOnly] private List<Gem> _gemsAvailableList = new();

        /// <summary>
        /// How much gems of each type is outside the bag (for ease of use)
        /// </summary>
        private Dictionary<GemType, int> _gemsAvailableCount;

        [SerializeField] private int _defaultDrawAmount = 3;

        public event Action<List<Gem>> OnContentModifiedEvent;
        public event Action<List<Gem>> OnGemsDrawn;

        public int DefaultDrawAmount
        {
            get => _defaultDrawAmount;
            set => _defaultDrawAmount = value;
        }

        public void Initialize(GemGroup gems)
        {
            _gemsAvailableCount = Enum.GetValues(typeof(GemType))
                .Cast<GemType>()
                .ToDictionary(type => type, _ => 0);

            _content.Clear();
            _gemsAvailableList.Clear();

            Add(GemType.Pearl, gems.Pearls);
            Add(GemType.Quartz, gems.Quartz);
            Add(GemType.Brimstone, gems.Brimstone);
        }

        private void Add(GemType gemType)
        {
            var gem = new Gem()
            {
                Type = gemType
            };

            _content.Add(gem);
            OnContentModifiedEvent?.Invoke(_content);
        }

        public void Add(GemType gemType, int count)
        {
            for (int i = 0; i < count; i++)
            {
                Add(gemType);
            }
        }

        /// <summary>
        /// Checks if there are at least a certain number of gems of a specific type available.
        /// </summary>
        /// <param name="gemType">The type of gem to check.</param>
        /// <param name="count">The number of gems required.</param>
        /// <returns>True if there are enough gems available; otherwise, false.</returns>
        public bool Has(GemType gemType, int count = 1)
        {
            return _gemsAvailableCount.TryGetValue(gemType, out int available) && available >= count;
        }

        /// <summary>
        /// Checks if there are at least the specified number of each gem type available.
        /// </summary>
        /// <param name="pearls">Number of pearls required.</param>
        /// <param name="quartz">Number of quartz required.</param>
        /// <param name="brimstone">Number of brimstones required.</param>
        /// <returns>True if the conditions are met; otherwise, false.</returns>
        public bool Has(int pearls, int quartz, int brimstone)
        {
            return Has(GemType.Pearl, pearls)
                   && Has(GemType.Quartz, quartz)
                   && Has(GemType.Brimstone, brimstone);
        }

        /// <summary>
        /// Draws multiple gems from the bag.
        /// </summary>
        public void Draw(int amount = 0)
        {
            amount = amount <= 0 ? DefaultDrawAmount : amount;

            var drawnGems = new List<Gem>();
            for (int i = 0; i < amount; i++)
            {
                if (_content.Count == 0) break; // Stop if bag is empty
                var gem = _content.SelectRandom();
                _content.Remove(gem);
                _gemsAvailableList.Add(gem);
                _gemsAvailableCount[gem.Type]++;
                drawnGems.Add(gem);
            }

            Debug.Log("Drawing new gems");

            OnGemsDrawn?.Invoke(drawnGems);
            OnContentModifiedEvent?.Invoke(_gemsAvailableList);
        }

        /// <summary>
        /// Reroll the content of the available gems
        /// </summary>
        public void Reroll()
        {
            var gemsToReturn = new List<Gem>(_gemsAvailableList);

            // Temporarily remove all available gems
            foreach (Gem gem in gemsToReturn)
            {
                RemoveGemFromAvailable(gem);
            }

            // Redraw new gems
            Draw(DefaultDrawAmount);

            // Return the removed gems to the content
            _content.AddRange(gemsToReturn);

            // Fire events to notify changes
            OnGemsDrawn?.Invoke(_gemsAvailableList);
            OnContentModifiedEvent?.Invoke(_content);
        }


        /// <summary>
        /// Draws a specific number of gems by type.
        /// </summary>
        public void Draw(GemType type, int amount)
        {
            var drawnGems = new List<Gem>();

            for (int i = 0; i < amount; i++)
            {
                var gemIndex = _content.FindIndex(g => g.Type == type);
                if (gemIndex == -1) break; // Stop if no gems of the specified type exist
                var gem = _content[gemIndex];
                _content.RemoveAt(gemIndex);
                _gemsAvailableList.Add(gem);
                _gemsAvailableCount[gem.Type]++;
                drawnGems.Add(gem);
            }

            OnGemsDrawn?.Invoke(drawnGems);
            OnContentModifiedEvent?.Invoke(_gemsAvailableList);
        }

        /// <summary>
        /// Returns a single gem to the bag.
        /// </summary>
        public void ReturnToBag(Gem gem)
        {
            if (_gemsAvailableList.Contains(gem))
            {
                RemoveGemFromAvailable(gem);
            }

            _content.Add(gem);
            OnContentModifiedEvent?.Invoke(_content);
        }

        /// <summary>
        /// Returns multiple gems to the bag by type and count.
        /// </summary>
        public void ReturnToBag(GemType type, int count)
        {
            for (int i = 0; i < count; i++)
            {
                var gemIndex = _gemsAvailableList.FindIndex(g => g.Type == type);
                if (gemIndex == -1) break; // Stop if no more gems of the type exist
                var gem = _gemsAvailableList[gemIndex];
                _gemsAvailableList.RemoveAt(gemIndex);
                _content.Add(gem);
                _gemsAvailableCount[gem.Type]--;
            }

            OnContentModifiedEvent?.Invoke(_content);
        }

        /// <summary>
        /// Destroys multiple gems of a specific type.
        /// </summary>
        public void DestroyGems(GemType type, int amount)
        {
            for (int i = 0; i < amount; i++)
            {
                var gemIndex = _gemsAvailableList.FindIndex(g => g.Type == type);
                if (gemIndex == -1) break; // Stop if no gems of the specified type exist
                var gem = _gemsAvailableList[gemIndex];
                DestroyGem(gem);
            }
        }

        private void DestroyGem(Gem gem)
        {
            gem.OnDestroy();

            if (_gemsAvailableList.Contains(gem))
            {
                RemoveGemFromAvailable(gem);
            }

            if (_content.Contains(gem))
            {
                _content.Remove(gem);
            }
        }

        private void RemoveGemFromAvailable(Gem gem)
        {
            _gemsAvailableCount[gem.Type] = Mathf.Max(0, _gemsAvailableCount[gem.Type] - 1);
            _gemsAvailableList.Remove(gem);
            Debug.Log($"Removed {gem.Type}");
        }
    }
}