using System.Collections.Generic;
using Runtime.CardGameplay.Card;
using Runtime.Combat.Pawn;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Runtime.RunManagement
{
    [CreateAssetMenu(fileName = "Player Class", menuName = "Game/Player Class", order = 0)]
    [Title("School Definition", TitleAlignment = TitleAlignments.Centered)]
    public class School : ScriptableObject
    {
        [TabGroup("Cards", "Starter")]
        [LabelText("Starter Cards")]
        [ListDrawerSettings(DraggableItems = true, ShowPaging = true)]
        [PropertyOrder(0)]
        [SerializeField]
        private List<CardData> _starterCards;

        [TabGroup("Cards", "Collectable")]
        [LabelText("Collectable Cards")]
        [ListDrawerSettings(DraggableItems = true, ShowPaging = true)]
        [PropertyOrder(1)]
        [SerializeField]
        private List<CardData> _collectableCards;

        [TabGroup("General")]
        [LabelText("Mage Unit")]
        [InlineEditor(ObjectFieldMode = InlineEditorObjectFieldModes.Foldout, Expanded = false)]
        [PropertyOrder(2)]
        [SerializeField]
        private PawnData _mage;

        // Exposed read‐only properties
        public List<CardData> StarterCards => _starterCards;
        public List<CardData> CollectableCards => _collectableCards;
        public PawnData Mage => _mage;
    }
}
