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
            if (source is not TooltipData data) return;

            _headerField.text = data.Header;
            _secondHeaderField.text = data.SecondHeader;
            _descriptionText.text = data.Description;
            _background.color = data.BackgroundColor;

            int headerLength = _headerField.text.Length;
            int secondHeaderLength = _secondHeaderField.text.Length;
            int contentLength = _descriptionText.text.Length;
            _layoutElement.enabled = headerLength > _characterWrapLimit
                                     || secondHeaderLength > _characterWrapLimit ||
                                     contentLength > _characterWrapLimit;

            var icon = data.Icon;
            if (icon)
            {
                _iconImage.sprite = icon;
                _iconImage.gameObject.SetActive(true);
            }
            else
            {
                _iconImage.gameObject.SetActive(false);
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