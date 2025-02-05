using Runtime.Combat.Pawn.Enemy;
using Runtime.Combat.Pawn.Targeting;
using Unity.Mathematics;
using UnityEngine;

namespace Runtime.Combat.Pawn
{
    public class PawnFactory : MonoBehaviour
    {
        [SerializeField] private GameObject _prefab;

        public PawnController Spawn(PawnData data)
        {
            var instance = Instantiate(_prefab, Vector3.zero, quaternion.identity);
            PawnController controller;
            if (data is EnemyData enemyData)
            {
                var enemyController = instance.AddComponent<EnemyController>();
                enemyController.Init(enemyData.PlayTable);
                enemyController.AddTargeting(PawnTargetType.Enemy);
                controller = enemyController;

                //TODO TEMP
                instance.GetComponent<SpriteRenderer>().flipX = true;
            }
            else
            {
                controller = instance.AddComponent<PawnController>().AddTargeting(PawnTargetType.Hero);
            }

            controller.Init(data);
            return controller;
        }
    }
}