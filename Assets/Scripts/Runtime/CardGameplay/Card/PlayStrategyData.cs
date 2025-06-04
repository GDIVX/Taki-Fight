using System;
using Runtime.CardGameplay.Card.CardBehaviour;
using Runtime;
using Sirenix.OdinInspector;

namespace Runtime.CardGameplay.Card
{
    [Serializable]
    public struct PlayStrategyData
    {
        [LabelText("Strategy")] public CardPlayStrategy PlayStrategy;

        [SerializeReference]
        [LabelText("Params")]
        public StrategyParams Parameters;

        [LabelText("Potency")] [MinValue(0)] public int Potency;
    }
}