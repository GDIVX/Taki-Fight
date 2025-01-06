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

        [Button]
        public void Add(Mana mana)
        {
            _inventory.Enqueue(mana);

            if (_inventory.Count > _capacity)
            {
                // Remove the last item from the inventory when over capacity
                _inventory.Dequeue();
            }

            CallOnInventoryModified();
        }


        [Button]
        public void SetCapacity(int value)
        {
            _capacity = value;
            OnCapacityModifiedEvent?.Invoke(_capacity);
        }

        [Button]
        public bool TryToExtractMana(List<Mana> cost)
        {
            // Can afford the cost?
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
                // Remove the first matching Mana from the inventory
                _inventory.Remove(mana);
            }

            CallOnInventoryModified();
        }

        public bool Contains(List<Mana> query)
        {
            // Create a temporary list to track the available mana
            var tempInventory = _inventory.ToList();

            foreach (var mana in query)
            {
                // Check if the query mana exists in the temporary inventory
                if (tempInventory.Contains(mana))
                {
                    tempInventory.Remove(mana); // Remove one instance of this mana
                }
                else
                {
                    return false; // Missing required mana
                }
            }

            return true; // All mana requirements met
        }
    }
}