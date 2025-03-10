using Runtime.Combat.Pawn;
using UnityEngine;

namespace Runtime.Combat
{
    public class CombatLane : MonoBehaviour
    {
        [SerializeField] private LaneSide _allySide, _enemySide;
        public LaneSide AllySide => _allySide;
        public LaneSide EnemySide => _enemySide;


        public void Clear()
        {
            AllySide.Clear();
            EnemySide.Clear();
        }
    }
}