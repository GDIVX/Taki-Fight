using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Runtime.CardGameplay.Card.View
{
    public class RewardCardView : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        [SerializeField] private TextMeshProUGUI _title;
        [SerializeField] private TextMeshProUGUI _description;
        [SerializeField] private Image _image;
        [SerializeField] private TextMeshProUGUI _costText;

        private System.Action<RewardCardView> _onSelect;

        private CardData _cardData;
        public bool IsHoverable { get; set; } = false;

        public void Init(CardData data, System.Action<RewardCardView> onSelect)
        {
            _cardData = data;
            _title.text = data.Title;
            _image.sprite = data.Image;

            _description.text = data.Description;

            _costText.text = data.Cost.ToString();

            _costText.transform.parent.gameObject.SetActive(data.Cost > 0);

            _onSelect = onSelect;
        }

        public CardData GetCardData() => _cardData;

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!IsHoverable)
            {
                return;
            }

            transform.DOScale(1.1f, 0.2f).SetEase(Ease.OutQuad);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            transform.DOScale(1f, 0.2f).SetEase(Ease.OutQuad);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (!IsHoverable)
            {
                return;
            }

            _onSelect?.Invoke(this);
        }
    }
}