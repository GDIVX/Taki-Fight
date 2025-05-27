using System.Collections.Generic;
using Runtime.CardGameplay.Card;
using Runtime.Combat.Pawn;
using UnityEngine;

namespace Runtime.RunManagement
{
    [CreateAssetMenu(fileName = "Player Class", menuName = "Game/Player Class", order = 0)]
    public class School : ScriptableObject
    {
        [SerializeField] private List<CardData> _starterCards;
        [SerializeField] private List<CardData> _collectableCards;
        [SerializeField] private PawnData _mage;

        public List<CardData> StarterCards => _starterCards;
        public List<CardData> CollectableCards => _collectableCards;
        public PawnData Mage => _mage;
    }
}