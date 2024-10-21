using Runtime.Combat.Pawn.Enemy;
using Unity.Mathematics;
using UnityEngine;

namespace Runtime.Combat.Pawn
{
    public class PawnFactory : MonoBehaviour
    {
        [SerializeField] private PawnController enemyPrefab;

        public PawnController SpawnEnemy(EnemyData data, Vector3 position)
        {
            var instance = GameObject.Instantiate(enemyPrefab, position, quaternion.identity);
            instance.Init(data);
            var ai = instance.GetComponent<EnemyController>();
            ai.Init(data.PlayTable);
            return instance;
        }
    }
}