using System;
using System.Collections.Generic;
using UnityEngine;

namespace Runtime.CardGameplay.ManaSystem
{
    [Serializable]
    public struct Mana
    {
        public List<ManaType> PossibleTypes;
        public Sprite Icon;

        public Mana(List<ManaType> possibleTypes, Sprite icon)
        {
            PossibleTypes = possibleTypes;
            Icon = icon;
        }

        public bool IsAny => PossibleTypes.Contains(ManaType.Any);

        public string DisplayName => string.Join("/", PossibleTypes);
    }
}