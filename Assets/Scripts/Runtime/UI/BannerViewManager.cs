using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEngine;

namespace Runtime.UI
{
    public class BannerViewManager : MonoBehaviour
    {
        [ShowInInspector] private List<BannerView> _banners;
        [SerializeField] private float _fadeDuration;
        [SerializeField] private ColorPalette _severityColorPalette;

        private void OnValidate()
        {
            _banners = new();
            foreach (BannerView banner in transform.GetComponentsInChildren<BannerView>())
            {
                _banners.Add(banner);
            }
        }

        private void Start()
        {
            Clear();
        }

        public void Clear()
        {
            foreach (var bannerView in _banners.Where(bannerView => bannerView.IsShowingMessage))
            {
                bannerView.Clear(_fadeDuration);
            }
        }

        public void WriteMessage(int bannerIndex, string message, int colorHighlight = 0)
        {

            colorHighlight = Mathf.Clamp(colorHighlight, 0, _severityColorPalette.Colors.Count);
            var textColor = _severityColorPalette.Colors[colorHighlight];

            var banner = _banners[bannerIndex];
            banner.SetMessage(message, textColor, _fadeDuration);
        }
    }
}