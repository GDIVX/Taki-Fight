using System;

namespace Runtime.CardGameplay.GemSystem
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