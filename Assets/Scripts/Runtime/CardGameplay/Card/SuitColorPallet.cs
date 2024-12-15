using System.Collections.Generic;
using UnityEngine;

namespace Runtime.CardGameplay.Card
{
    [CreateAssetMenu(fileName = "SuitColorPallet", menuName = "SuitColorPallet", order = 0)]
    public class SuitColorPallet : ScriptableObject
    {
        [SerializeField] private List<SuitColor> pallet;

        public Color GetColor(CardGlyph cardGlyph)
        {
            return pallet.Find(p => p.CardGlyph == cardGlyph).color;
        }
    }
}