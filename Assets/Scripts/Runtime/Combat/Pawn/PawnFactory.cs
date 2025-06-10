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

        /// <summary>
        ///     Instantiate a pawn on a tile and initialize it.
        /// </summary>
        /// <param name="unit">Scriptable Object that defines the pawn</param>
        /// <param name="tile">The tile at which to position the pawn</param>
        /// <returns></returns>
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

        internal PawnController CreatePawn(PawnInstant instant, Tile tile)
        {
            if (instant == null) return null;
            if (instant.CurrentHealth <= 0) return null;

            var data = instant.Data;
            var controller = CreatePawn(data, tile);
            controller.SetInstant(instant);
            controller.Health.SetHealth(instant.CurrentHealth);
            return controller;
        }
    }
}