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
        [SerializeField] private Image suite;
        [SerializeField] private SuitColorPallet colorPallet;

        [Button]
        public void Draw(CardData data, int number)
        {
            title.text = data.Title;
            description.text = data.Description;
            numberText.text = number.ToString();
            image.sprite = data.Image;
            suite.color = colorPallet.GetColor(data.Suit);
        }
    }
}