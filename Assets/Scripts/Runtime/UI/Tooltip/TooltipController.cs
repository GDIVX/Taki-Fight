using System;
using DG.Tweening;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Runtime.UI.Tooltip
{
    [ExecuteInEditMode]
    public class TooltipController : MonoBehaviour
    {
        [SerializeField] private TMP_Text _headerField;
        [SerializeField] private TMP_Text _secondHeaderField;
        [SerializeField] private TMP_Text _descriptionText;
        [SerializeField] private Image _iconImage;
        [SerializeField] private Image _background;
        [SerializeField] private LayoutElement _layoutElement;
        [SerializeField] private int _characterWrapLimit;
        [SerializeField] private Vector2 _pointerOffset = new Vector2(15f, -15f);

        private RectTransform _rectTransform;

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
        }

        public void SetTooltip(string title, string secondHeader = "", string description = "",
            Color backgroundColor = default,
            Sprite icon = null)
        {
            _headerField.text = title;
            _secondHeaderField.text = secondHeader;
            _descriptionText.text = description;
            _background.color = backgroundColor;

            int headerLength = _headerField.text.Length;
            int secondHeaderLength = _secondHeaderField.text.Length;
            int contentLength = _descriptionText.text.Length;
            _layoutElement.enabled = headerLength > _characterWrapLimit
                                     || secondHeaderLength > _characterWrapLimit ||
                                     contentLength > _characterWrapLimit;

            if (icon != null)
            {
                _iconImage.sprite = icon;
                _iconImage.gameObject.SetActive(true);
            }
            else
            {
                _iconImage.gameObject.SetActive(false);
            }
        }

        private void CalculatePosition()
        {
            Vector2 mousePosition = Input.mousePosition;
            float pivotX = mousePosition.x / Screen.width;
            float pivotY = mousePosition.y / Screen.height;
            _rectTransform.pivot = new Vector2(pivotX, pivotY);

            // Apply offset
            Vector2 newPosition = mousePosition + _pointerOffset;

            // Determine tooltip’s scaled size
            float scaledWidth = _rectTransform.rect.width * _rectTransform.lossyScale.x;
            float scaledHeight = _rectTransform.rect.height * _rectTransform.lossyScale.y;

            // Clamp so we stay fully on-screen
            newPosition.x = Mathf.Clamp(newPosition.x, 0, Screen.width - scaledWidth);
            newPosition.y = Mathf.Clamp(newPosition.y, scaledHeight, Screen.height);

            transform.position = newPosition;
        }

        public void ShowTooltip()
        {
            CalculatePosition();
            gameObject.SetActive(true); // turn on first
        }

        public void HideTooltip()
        {
            gameObject.SetActive(false);
        }

        public void Reset()
        {
            _headerField.text = string.Empty;
            _descriptionText.text = string.Empty;
            _iconImage.sprite = null;
            _iconImage.gameObject.SetActive(false);
        }
    }
}