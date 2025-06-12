using System;
using UnityEngine;

namespace Runtime.CardGameplay.Card.CardBehaviour
{
    [CreateAssetMenu(fileName = "Conditional Play", menuName = "Card/Strategy/Play/Conditional", order = 0)]
    public class ConditionalCardPlayStrategy : CardPlayStrategy
    {
        [SerializeField] private CardPlayStrategy mainStrategy;
        [SerializeField] private CardPlayStrategy followUpStrategy;
        [SerializeField] private string conditionEvent;

        public CardPlayStrategy MainStrategy => mainStrategy;
        public CardPlayStrategy FollowUpStrategy => followUpStrategy;
        public string ConditionEvent => conditionEvent;

        public override void Play(CardController cardController, Action<CardPlayResult> onComplete)
        {
            if (mainStrategy == null)
            {
                onComplete?.Invoke(new CardPlayResult(false));
                return;
            }

            mainStrategy.Play(cardController, result =>
            {
                if (result.IsResolved && result.EventType == conditionEvent && followUpStrategy != null)
                {
                    followUpStrategy.Play(cardController, _ => onComplete?.Invoke(result));
                }
                else
                {
                    onComplete?.Invoke(result);
                }
            });
        }

        public override string GetDescription()
        {
            var mainDesc = mainStrategy ? mainStrategy.GetDescription() : string.Empty;
            var followDesc = followUpStrategy ? followUpStrategy.GetDescription() : string.Empty;
            return string.IsNullOrEmpty(followDesc) ? mainDesc : $"{mainDesc} then {followDesc}";
        }
    }
}

