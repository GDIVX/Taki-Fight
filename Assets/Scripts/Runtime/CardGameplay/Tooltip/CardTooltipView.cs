using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Runtime.CardGameplay.Tooltip
{
    public class CardTooltipView : MonoBehaviour
    {
        [SerializeField] private Image _boundingBoxImage;
        [SerializeField] private TextMeshProUGUI _headerField;
        [SerializeField] private TextMeshProUGUI _contentField;
        [SerializeField] private LayoutElement _layoutElement;
        [SerializeField] private int _characterWarpLimit;

        private void Update()
        {
            int headerLength = _headerField.text.Length;
            int contextLength = _contentField.text.Length;

            _layoutElement.enabled = headerLength > _characterWarpLimit || contextLength > _characterWarpLimit;
        }

        public void Draw(TooltipData data)
        {
            _boundingBoxImage.sprite = data.BoundingBoxTexture;
            _boundingBoxImage.color = data.Color;

            _headerField.text = data.HeaderText;
            _contentField.text = data.ContentText;

            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}