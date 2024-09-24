using UnityEngine;

namespace Runtime.CardGameplay.Card.CardBehaviour
{
    [CreateAssetMenu(fileName = "Print Message Play", menuName = "Card/Strategy/Play/Print Message", order = 0)]
    public class PrintMessageOnPlay : CardPlayStrategy
    {
        public override void Play(CardController card)
        {
            Debug.Log($"Played card {card.Suit} {card.Number}");
        }
    }
}