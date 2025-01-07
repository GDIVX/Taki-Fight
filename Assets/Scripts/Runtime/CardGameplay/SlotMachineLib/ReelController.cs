using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Runtime.CardGameplay.SlotMachineLib
{
    public class ReelController : MonoBehaviour
    {
        [ShowInInspector, ReadOnly] private List<SMSymbol> _slots = new List<SMSymbol>();
        private int CurrentIndex { get; set; }

        [ShowInInspector, ReadOnly] public SMSymbol CurrentSymbol { get; private set; }

        public event Action<ReelController> OnSpin;

        public void SetupSlots(List<SMSymbol> slots)
        {
            _slots = new List<SMSymbol>(slots);
        }

        [Button]
        public void Spin()
        {
            if (_slots.Count == 0) return;

            CurrentIndex = Random.Range(0, _slots.Count);
            UpdateAndExecuteCurrentSymbol();
        }

        private void UpdateAndExecuteCurrentSymbol()
        {
            CurrentSymbol = _slots[CurrentIndex];
            CurrentSymbol?.Execute();
            OnSpin?.Invoke(this);
        }
    }
}