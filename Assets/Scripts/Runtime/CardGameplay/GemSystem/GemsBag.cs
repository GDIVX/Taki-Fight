using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using Utilities;

namespace Runtime.CardGameplay.GemSystem
{
    public class GemsBag : MonoBehaviour
    {
        [ShowInInspector, ReadOnly] private Dictionary<GemType, int> _gems;

        [SerializeField, Required] private BagView _view;

        public IReadOnlyDictionary<GemType, int> Gems => _gems;
        public event Action OnModifiedEvent;


        public void Initialize()
        {
            _gems = new Dictionary<GemType, int>()
            {
                [GemType.Pearl] = 0,
                [GemType.Quartz] = 0,
                [GemType.Brimstone] = 0
            };

            // Fire events once
            OnModifiedEvent?.Invoke();

            // Initialize the view (UI or visual representation)
            _view.Initialize(this);
        }


        public void OnTurnEnd()
        {
            _gems[GemType.Pearl] = 0;
            _gems[GemType.Quartz] = 0;
            _gems[GemType.Brimstone] = 0;
        }

        public void Add(GemType gemType, int count)
        {
            _gems[gemType] += count;

            OnModifiedEvent?.Invoke();
        }

        public void Remove(GemType gemType, int count)
        {
            _gems[gemType] = Mathf.Max(0, _gems[gemType] - count);
            OnModifiedEvent?.Invoke();
        }


        #region Helpers / Checks

        public int CountAvailable(GemType gemType)
        {
            return _gems[gemType];
        }

        private bool Has(GemType gemType, int need = 1)
        {
            return CountAvailable(gemType) >= need;
        }

        public bool Has(int pearls, int quartz, int brimstone)
        {
            var hasPeals = Has(GemType.Pearl, pearls);
            var hasQuartz = Has(GemType.Quartz, quartz);
            var hasBrimstone = Has(GemType.Brimstone, brimstone);

            return hasPeals && hasQuartz && hasBrimstone;
        }

        #endregion
    }
}