using System.Collections.Generic;
using UnityEngine;

namespace Runtime.CardGameplay.ManaSystem
{
    [CreateAssetMenu(fileName = "ManaDefinition", menuName = "Game/ManaDefinition", order = 0)]
    public class ManaDefinition : ScriptableObject
    {
        [SerializeField] private List<ManaType> _possibleTypes; // Enum-based types
        [SerializeField] private Sprite _icon;

        public Mana InstantiateMana()
        {
            return new Mana(new List<ManaType>(_possibleTypes), _icon);
        }
    }
}