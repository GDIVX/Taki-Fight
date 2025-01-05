using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Runtime.CardGameplay.SlotMachineLib
{
    public class SlotMachine : MonoBehaviour
    {
        [SerializeField] private Transform _reelsParentTransform;
        [SerializeField] private ReelController _reelPrefab;
        [ShowInInspector, ReadOnly] private List<ReelController> _reels;

        [Button]
        public void Initialize(int reelCounts, ReelDefinition reelDefinition)
        {
            var slots = reelDefinition.CreateSlots();
            _reels = new List<ReelController>();
            for (int i = 0; i < reelCounts; i++)
            {
                var reelInstance = Instantiate(_reelPrefab, _reelsParentTransform);
                reelInstance.Initialize(slots);
                _reels.Add(reelInstance);
            }
        }

        [Button]
        public void Spin()
        {
            _reels.ForEach(reel => reel.Spin());
        }
    }
}