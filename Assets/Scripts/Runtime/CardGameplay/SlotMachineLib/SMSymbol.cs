using System;
using Runtime.CardGameplay.Card;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Runtime.CardGameplay.SlotMachineLib
{
    [Serializable]
    public class SMSymbol
    {
        [ShowInInspector, ReadOnly] private PlayStrategyData _strategyData;

        [ShowInInspector, ReadOnly] public Sprite Sprite { get; private set; }

        public SMSymbol(PlayStrategyData playStrategyData, Sprite sprite)
        {
            _strategyData = playStrategyData;
            Sprite = sprite;
        }

        public static SMSymbol Create(SymbolData data)
        {
            return new(data.PlayStrategyData, data.Sprite);
        }

        public void Execute()
        {
            var playStrategy = _strategyData.PlayStrategy;
            var potency = _strategyData.Potency;

            playStrategy.Play(GameManager.Instance.Hero, potency);
        }
    }
}