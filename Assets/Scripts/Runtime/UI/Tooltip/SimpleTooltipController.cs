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
            if (source is not IContentTooltipSource tooltipSource)
                return;

            if (!_headerField || !_secondHeaderField || !_descriptionText ||
                !_iconImage || !_background || !_layoutElement)
            {
                Debug.LogError("[SimpleTooltipController] Missing serialized field(s) in the Inspector.");
                return;
            }

            _headerField.text = tooltipSource.Header ?? string.Empty;
            _secondHeaderField.text = tooltipSource.Subtitle ?? string.Empty;
            _descriptionText.text = tooltipSource.Content ?? string.Empty;
            _background.color = tooltipSource.BackgroundColor;

            int headerLength = _headerField.text?.Length ?? 0;
            int secondHeaderLength = _secondHeaderField.text?.Length ?? 0;
            int contentLength = _descriptionText.text?.Length ?? 0;

            _layoutElement.enabled = headerLength > _characterWrapLimit
                                     || secondHeaderLength > _characterWrapLimit
                                     || contentLength > _characterWrapLimit;

            if (tooltipSource.Icon)
            {
                _iconImage.sprite = tooltipSource.Icon;
                _iconImage.gameObject.SetActive(true);
            }
            else
            {
                _iconImage.sprite = null;
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