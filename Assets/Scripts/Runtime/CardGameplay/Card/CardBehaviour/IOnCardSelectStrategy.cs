namespace Runtime.CardGameplay.Card.CardBehaviour
{
    public interface IOnCardSelectStrategy
    {
        /// <summary>
        /// Handle the selection logic of a card
        /// </summary>
        /// <returns></returns>
        public bool OnSelect();
    }
}