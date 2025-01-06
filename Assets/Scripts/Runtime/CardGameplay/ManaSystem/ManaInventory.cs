using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using Utilities;

namespace Runtime.CardGameplay.ManaSystem
{
    public class ManaInventory : MonoBehaviour
    {
        [SerializeField] private int _capacity;
        [ShowInInspector, ReadOnly] private FlexibleQueue<Mana> _inventory = new();

        public event Action<List<Mana>> OnInventoryModifiedEvent;
        public event Action<int> OnCapacityModifiedEvent;

        private const string WildName = "wild";

        public int Capacity => _capacity;
        public List<Mana> Inventory => _inventory.ToList();

        [Button]
        public void Add(Mana mana)
        {
            _inventory.Enqueue(mana);

            if (_inventory.Count > _capacity)
            {
                // Remove excess mana
                var removedMana = _inventory.Dequeue();
                Debug.Log($"Mana {removedMana.Name} removed due to capacity limit.");
            }

            CallOnInventoryModified();
        }

        [Button]
        public void SetCapacity(int value)
        {
            _capacity = value;
            OnCapacityModifiedEvent?.Invoke(_capacity);

            // Trim inventory if capacity is reduced
            while (_inventory.Count > _capacity)
            {
                var removedMana = _inventory.Dequeue();
                Debug.Log($"Mana {removedMana.Name} removed due to reduced capacity.");
            }

            CallOnInventoryModified();
        }

        [Button]
        public bool TryToExtractMana(List<Mana> cost)
        {
            if (!Contains(cost)) return false;

            Extract(cost);
            return true;
        }

        private void CallOnInventoryModified()
        {
            OnInventoryModifiedEvent?.Invoke(_inventory.ToList());
        }

        private void Extract(List<Mana> cost)
        {
            foreach (var mana in cost)
            {
                if (mana.Name == WildName)
                {
                    // Wild mana consumes the first available mana
                    _inventory.Dequeue();
                }
                else
                {
                    // Remove the first matching mana by type
                    for (int i = 0; i < _inventory.Count; i++)
                    {
                        if (_inventory.ElementAt(i).Name == mana.Name)
                        {
                            _inventory.RemoveAt(i);
                            break;
                        }
                    }
                }
            }

            CallOnInventoryModified();
        }

        public bool Contains(List<Mana> query)
        {
            var tempInventory = _inventory.ToList();

            foreach (var mana in query)
            {
                if (mana.Name == WildName)
                {
                    if (tempInventory.Count > 0)
                    {
                        tempInventory.RemoveAt(0); // Use the first available mana
                        continue;
                    }

                    return false; // No mana left for Wild to use
                }

                // Check for matching mana
                bool matchFound = false;
                for (int i = 0; i < tempInventory.Count; i++)
                {
                    if (tempInventory[i].Name == mana.Name)
                    {
                        tempInventory.RemoveAt(i);
                        matchFound = true;
                        break;
                    }
                }

                if (!matchFound)
                {
                    return false; // Required mana not found
                }
            }

            return true;
        }
    }
}