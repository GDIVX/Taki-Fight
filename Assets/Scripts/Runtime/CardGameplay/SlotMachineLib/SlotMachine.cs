using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Runtime.CardGameplay.SlotMachineLib
{
    public class SlotMachine : MonoBehaviour
    {
        [SerializeField] private Transform _reelsParentTransform;
        [SerializeField] private ReelController _reelPrefab;
        [SerializeField] private float _waitTime = 0.5f;
        [ShowInInspector, ReadOnly] private List<ReelController> _reels;

        [SerializeField] private SlotMachineView _slotMachineView;

        public event Action OnSpin;
        public event Action OnComplete;

        [Button]
        public void Initialize(int reelCounts)
        {
            _reels = new List<ReelController>();
            for (int i = 0; i < reelCounts; i++)
            {
                var reelInstance = Instantiate(_reelPrefab, _reelsParentTransform);
                _reels.Add(reelInstance);
            }
        }

        public void SetupSlots(ReelDefinition reelDefinition)
        {
            foreach (ReelController reel in _reels)
            {
                var slots = reelDefinition.CreateSlots();
                reel.SetupSlots(slots);
            }
        }

        /// <summary>
        /// Entry point for the entire slot machine sequence.
        /// </summary>
        [Button]
        public SlotMachine Spin()
        {
            // Kick off the sequence
            StartCoroutine(SpinSequence());
            return this;
        }

        private IEnumerator SpinSequence()
        {
            // 1) Show the UI
            _slotMachineView.Show();
            // Wait until the Show animation completes
            yield return _slotMachineView.WaitForTween();

            // Optional short wait
            yield return new WaitForSeconds(_waitTime);

            // 2) Start spinning the reels
            _reels.ForEach(r => r.Spin());
            OnSpin?.Invoke(); // Fire the OnSpin event if other systems are listening

            // 3) Optionally wait while reels spin or do something in parallel
            yield return new WaitForSeconds(2f); // Example wait time, adjust as needed

            // 4) Hide the UI once everything is done
            _slotMachineView.Hide();
            yield return _slotMachineView.WaitForTween();
            OnComplete?.Invoke();
        }
    }
}