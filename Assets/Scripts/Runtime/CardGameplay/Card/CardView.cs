using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Runtime.CardGameplay.Card
{
    public class CardView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI title;
        [SerializeField] private TextMeshProUGUI description;
        [SerializeField] private TextMeshProUGUI numberText;
        [SerializeField] private Image image;
        [SerializeField] private Image suitImage;
        [SerializeField] private SuitColorPallet colorPallet;

        [Button]
        public void Draw(CardData data, int number, Suit suit)
        {
            if (suit == Suit.White)
            {
                numberText.text = "J";
                numberText.color = colorPallet.GetColor(Suit.Black);
            }
            else
            {
                numberText.text = number.ToString();
                numberText.color = colorPallet.GetColor(Suit.White);
            }

            title.text = data.Title;
            description.text = data.Description;
            image.sprite = data.Image;

            var color = colorPallet.GetColor(suit);
            title.color = color;
            description.color = color;
            suitImage.color = color;
            //TODO image.color = color; 
        }
    }
}