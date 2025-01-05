using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Runtime.CardGameplay.SlotMachineLib
{
    [CreateAssetMenu(fileName = "Reel Definition" , menuName = "Slot Machine/Reel")]
    public class ReelDefinition : ScriptableObject
    {
        public List<SymbolData> SymbolsDatabase;

        public List<SMSymbol> CreateSlots()
        {
            var slots = new List<SMSymbol>();
            if (slots == null) throw new ArgumentNullException(nameof(slots));

            slots.AddRange(SymbolsDatabase.Select(SMSymbol.Create));

            return slots;
        }
    }
}