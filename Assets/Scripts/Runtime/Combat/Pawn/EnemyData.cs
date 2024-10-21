using Runtime.Combat.Pawn.Enemy;
using UnityEngine;

namespace Runtime.Combat.Pawn
{
    [CreateAssetMenu(menuName = "Pawns/Enemy", fileName = "EnemyData", order = 0)]
    public class EnemyData : PawnData
    {
        [SerializeField] private AIPlayTable playTable;

        public AIPlayTable PlayTable => playTable;
    }
}