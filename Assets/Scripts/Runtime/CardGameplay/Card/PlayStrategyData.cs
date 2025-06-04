using System;
using Runtime.CardGameplay.Card.CardBehaviour;
using Sirenix.OdinInspector;

namespace Runtime.CardGameplay.Card
{
    [Serializable]
    public struct PlayStrategyData
    {
        [LabelText("Strategy")] public CardPlayStrategy PlayStrategy;

        [LabelText("Potency")] [MinValue(0)] public int Potency;
    }
}