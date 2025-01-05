using UnityEngine;

namespace Runtime.CardGameplay.ManaSystem
{
    [CreateAssetMenu(fileName = "Mana", menuName = "Game/Mana", order = 0)]
    public class ManaDefinition : ScriptableObject
    {
        [SerializeField] private Sprite _icon;
        [SerializeField] private string _name;
        private string Name => _name;
        private Sprite Icon => _icon;

        public Mana InstantiateMana()
        {
            return new Mana(Name, Icon);
        }
    }
}