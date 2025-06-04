using Runtime.Combat.Tilemap;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utilities;

namespace Runtime.Combat.Pawn
{
    public class PawnFactory : MonoService<PawnFactory>
    {
        [SerializeField] private GameObject _prefab;

        internal PawnController CreatePawn(PawnData unit, Tile tile)
        {
            var instance = Instantiate(_prefab, Vector3.zero, quaternion.identity);
            var controller = instance.GetComponent<PawnController>();

            if (!controller)
            {
                Debug.LogError("Pawn prefab is missing PawnController component.");
                Destroy(instance);
                return null;
            }

            unit.InitializeStrategies();
            controller.Init(unit);
            controller.SpawnAtPosition(tile);
            SceneManager.MoveGameObjectToScene(controller.gameObject, SceneManager.GetSceneByName("Combat"));

            return controller;
        }
    }
}