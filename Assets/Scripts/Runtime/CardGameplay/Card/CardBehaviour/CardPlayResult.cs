namespace Runtime.CardGameplay.Card.CardBehaviour
{
    public class CardPlayResult
    {
        public bool IsResolved { get; }
        public string EventType { get; }

        public CardPlayResult(bool isResolved, string eventType = null)
        {
            IsResolved = isResolved;
            EventType = eventType;
        }
    }
}

