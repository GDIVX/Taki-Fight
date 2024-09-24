using TMPro;
using UnityEngine;

namespace Runtime.CardGameplay.Deck
{
    public class DeckView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI drawPileCounter;
        [SerializeField] private TextMeshProUGUI discardPileCounter;

        private Deck _deck;

        public void Setup(Deck deck)
        {
            _deck = deck;

            _deck.OnDrawPileUpdated += pile => drawPileCounter.text = pile.Count.ToString();
            _deck.OnDiscardPileUpdated += pile => discardPileCounter.text = pile.Count.ToString();
        }
    }
}