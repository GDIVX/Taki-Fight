using Runtime.Combat.Pawn.Enemy;
using Unity.Mathematics;
using UnityEngine;

namespace Runtime.Combat.Pawn
{
    public class PawnFactory : MonoBehaviour
    {
        [SerializeField] private PawnController enemyPrefab;

        public PawnController SpawnEnemy(EnemyData data)
        {
            var instance = GameObject.Instantiate(enemyPrefab, Vector3.zero, quaternion.identity);
            instance.Init(data);
            var ai = instance.GetComponent<EnemyController>();
            ai.Init(data.PlayTable);
            return instance;
        }
    }
}