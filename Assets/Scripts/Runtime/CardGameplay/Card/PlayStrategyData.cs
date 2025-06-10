using System;
using Runtime.CardGameplay.Card.CardBehaviour;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Runtime.CardGameplay.Card
{
    [Serializable]
    public struct PlayStrategyData
    {
        [LabelText("Strategy")] public CardPlayStrategy PlayStrategy;

        [SerializeReference] [LabelText("Params")]
        public StrategyParams Parameters;

        [LabelText("Potency")] [MinValue(0)] public int Potency;
    }
}
