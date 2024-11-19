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
        [ShowInInspector] private Dictionary<string, BannerView> _banners;
        [SerializeField] private float _fadeDuration;
        [SerializeField] private ColorPalette _severityColorPalette;

        private void OnValidate()
        {
            _banners = new Dictionary<string, BannerView>();
            foreach (BannerView banner in transform.GetComponentsInChildren<BannerView>())
            {
                _banners[banner.name] = banner;
            }
        }

        private void Start()
        {
            Clear();
        }

        public void Clear()
        {
            foreach (var bannerView in _banners.Values.Where(bannerView => bannerView.IsShowingMessage))
            {
                bannerView.Clear(_fadeDuration);
            }
        }

        public void WriteMessage(string bannerKey, string message, int severity = 0)
        {
            if (!_banners.ContainsKey(bannerKey))
            {
                throw new KeyNotFoundException(
                    $"failed to find a banner with the key '{bannerKey}'. Check the name of the banners.");
            }

            severity = Mathf.Clamp(severity, 0, _severityColorPalette.Colors.Count);
            var textColor = _severityColorPalette.Colors[severity];

            var banner = _banners[bannerKey];
            banner.SetMessage(message, textColor, _fadeDuration);
        }
    }
}