using System;
using UnityEngine;

namespace Runtime.CardGameplay.ManaSystem
{
    [Serializable]
    public struct Mana
    {
        public string Name;
        public Sprite Icon;

        public Mana(string name, Sprite icon)
        {
            Name = name;
            Icon = icon;
        }
    }
}