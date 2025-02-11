using System.Collections.Generic;
using Runtime.CardGameplay.Card;
using Runtime.Combat.Pawn;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Runtime.RunManagement
{
    [CreateAssetMenu(fileName = "Player Class", menuName = "Game/Player Class", order = 0)]
    public class PlayerClassData : ScriptableObject
    {
        [SerializeField] private List<CardData> _starterCards;
        [SerializeField] private List<CardData> _collectableCards;
        [SerializeField]  private PawnData _pawn;
    }
}