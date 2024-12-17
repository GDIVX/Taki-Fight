using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Runtime.CardGameplay.Tooltip
{
    public class CardTooltipView : MonoBehaviour
    {
        [SerializeField] private Image _boundingBoxImage;
        [SerializeField] private TextMeshProUGUI _headerGUI;
        [SerializeField] private TextMeshProUGUI _contentGUI;

        public void Draw(TooltipData data)
        {
            _boundingBoxImage.sprite = data.BoundingBoxTexture;
            _boundingBoxImage.color = data.Color;

            _headerGUI.text = data.HeaderText;
            _contentGUI.text = data.ContentText;

            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}