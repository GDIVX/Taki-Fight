using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Utilities;

namespace Runtime.UI
{
    public class BannerViewManager : MonoService<BannerViewManager>
    {
        [SerializeField] private float _fadeDuration;
        [ShowInInspector] private List<BannerView> _banners;

        private void Awake()
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
            foreach (var bannerView in _banners)
            {
                bannerView.Clear(_fadeDuration);
            }
        }

        public void WriteMessage(int bannerIndex, string message, Color textColor)
        {
            // severity = Mathf.Clamp(severity, 0, _severityColorPalette.Colors.Count);
            // var textColor = _severityColorPalette.Colors[severity];

            var banner = _banners[bannerIndex];
            banner.SetMessage(message, textColor, _fadeDuration);
        }
    }
}