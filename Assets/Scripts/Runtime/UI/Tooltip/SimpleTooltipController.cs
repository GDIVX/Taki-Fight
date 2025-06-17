using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Runtime.UI.Tooltip
{
    [ExecuteInEditMode]
    public class SimpleTooltipController : TooltipController
    {
        [SerializeField] private TMP_Text _headerField;
        [SerializeField] private TMP_Text _secondHeaderField;
        [SerializeField] private TMP_Text _descriptionText;
        [SerializeField] private Image _iconImage;
        [SerializeField] private Image _background;
        [SerializeField] private LayoutElement _layoutElement;
        [SerializeField] private int _characterWrapLimit;


        public override void SetTooltip(ITooltipSource source)
        {
            if (source is not IContentTooltipSource tooltipSource) return;

            _headerField.text = tooltipSource.Header;
            _secondHeaderField.text = tooltipSource.Subtitle;
            _descriptionText.text = tooltipSource.Content;
            _background.color = tooltipSource.BackgroundColor;

            int headerLength = _headerField ? _headerField.text.Length : 0;
            int secondHeaderLength = _secondHeaderField ? _secondHeaderField.text.Length : 0;
            int contentLength = _descriptionText ? _descriptionText.text.Length : 0;
            _layoutElement.enabled = headerLength > _characterWrapLimit
                                     || secondHeaderLength > _characterWrapLimit ||
                                     contentLength > _characterWrapLimit;

            var icon = tooltipSource.Icon;
            if (icon)
            {
                _iconImage.sprite = icon;
                _iconImage?.gameObject.SetActive(true);
            }
            else
            {
                _iconImage?.gameObject.SetActive(false);
            }
        }


        public override void ShowTooltip()
        {
            CalculatePosition();
            gameObject.SetActive(true); // turn on first
        }

        public override void HideTooltip()
        {
            gameObject.SetActive(false);
        }

        public override void Reset()
        {
            _headerField.text = string.Empty;
            _descriptionText.text = string.Empty;
            _iconImage.sprite = null;
            _iconImage.gameObject.SetActive(false);
        }
    }
}