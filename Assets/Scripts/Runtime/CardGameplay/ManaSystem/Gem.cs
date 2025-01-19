using System;
using System.Collections.Generic;
using UnityEngine;

namespace Runtime.CardGameplay.ManaSystem
{
    [Serializable]
    public struct Gem
    {
        public GemType Type;

        //TODO: Enchantments
        public void OnDestroy()
        {
            //TODO
        }
    }
}