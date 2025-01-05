using Runtime.CardGameplay.Card;
using UnityEngine;

namespace Runtime.CardGameplay.SlotMachineLib
{
    [CreateAssetMenu(fileName = "SymbolData", menuName = "Slot Machine/Symbol/Data", order = 0)]
    public class SymbolData : ScriptableObject
    {
        [SerializeField] private Sprite _sprite;
        [SerializeField] private PlayStrategyData _playStrategyData;
        

        public PlayStrategyData PlayStrategyData => _playStrategyData;

        public Sprite Sprite => _sprite;
    }
}