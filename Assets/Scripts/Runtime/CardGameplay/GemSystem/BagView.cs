using System;
using TMPro;
using UnityEngine;

namespace Runtime.CardGameplay.GemSystem
{
    public class BagView : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI _bagContentCount, _pearlsCount, _quartzCount, _brimstoneCount, _gemPerDrawCount;

        public void Initialize(GemsBag gemsBag)
        {
            _bagContentCount.text = gemsBag.Count.ToString();
            gemsBag.OnContentModifiedEvent += gems => _bagContentCount.text = gems.Count.ToString();

            UpdateGemsCount(gemsBag);
            gemsBag.OnAvailableModified += (_) => UpdateGemsCount(gemsBag);
        }

        private void UpdateGemsCount(GemsBag gemsBag)
        {
            _pearlsCount.text = gemsBag.CountAvailable(GemType.Pearl).ToString();
            _quartzCount.text = gemsBag.CountAvailable(GemType.Quartz).ToString();
            _brimstoneCount.text = gemsBag.CountAvailable(GemType.Brimstone).ToString();
        }
    }
}