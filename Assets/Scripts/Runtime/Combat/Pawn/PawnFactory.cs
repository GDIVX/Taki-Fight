using Runtime.Combat.Tilemap;
using Unity.Mathematics;
using UnityEngine;
using Utilities;

namespace Runtime.Combat.Pawn
{
    public class PawnFactory : MonoService
    {
        [SerializeField] private GameObject _prefab;

        internal PawnController CreatePawn(PawnData unit, Tile tile)
        {
            var instance = Instantiate(_prefab, Vector3.zero, quaternion.identity);
            var controller = instance.GetComponent<PawnController>();

            if (controller == null)
            {
                Debug.LogError("Pawn prefab is missing PawnController component.");
                Destroy(instance);
                return null;
            }

            controller.Init(unit);
            controller.SpawnAtPosition(tile);

            return controller;
        }
    }
}