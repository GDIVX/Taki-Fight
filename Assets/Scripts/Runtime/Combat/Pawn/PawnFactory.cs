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

            controller = instance.AddComponent<PawnController>();

            controller.Init(data);
            return controller;
        }
    }
}