using Sirenix.OdinInspector;
using UnityEngine;

namespace Runtime.CardGameplay.Card
{
    [CreateAssetMenu(fileName = "CardData", menuName = "Card", order = 0)]
    public class CardData : ScriptableObject
    {
        [SerializeField] private string title;
        [SerializeField] private string description;
        [SerializeField] private Sprite image;
        [SerializeField] private Suit suit;

        public string Title => title;
        public string Description => description;
        public Sprite Image => image;

        public Suit Suit => suit;
    }
}