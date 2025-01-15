using System;
using System.Collections.Generic;
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
                Debug.Log($"Mana {removedMana.DisplayName} removed due to capacity limit.");
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
                Debug.Log($"Mana {removedMana.DisplayName} removed due to reduced capacity.");
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

        public void Extract(List<Mana> cost)
        {
            foreach (var manaNeeded in cost)
            {
                if (manaNeeded.IsAny)
                {
                    _inventory.Dequeue();
                }
                else
                {
                    foreach (var type in manaNeeded.PossibleTypes)
                    {
                        for (int i = 0; i < _inventory.Count; i++)
                        {
                            if (_inventory.ElementAt(i).PossibleTypes.Contains(type))
                            {
                                _inventory.RemoveAt(i);
                                break;
                            }
                        }
                    }
                }
            }

            CallOnInventoryModified();
        }

        public bool Contains(List<Mana> query)
        {
            var tempInventory = _inventory.ToList();

            foreach (var manaNeeded in query)
            {
                if (manaNeeded.IsAny)
                {
                    if (tempInventory.Count == 0)
                        return false; // No mana available
                    tempInventory.RemoveAt(0); // Use any mana
                }
                else
                {
                    bool matchFound = false;

                    foreach (var type in manaNeeded.PossibleTypes)
                    {
                        for (int i = 0; i < tempInventory.Count; i++)
                        {
                            if (tempInventory[i].PossibleTypes.Contains(type))
                            {
                                tempInventory.RemoveAt(i); // Match found
                                matchFound = true;
                                break;
                            }
                        }

                        if (matchFound) break;
                    }

                    if (!matchFound)
                        return false; // Required type not found
                }
            }

            return true;
        }
    }
}