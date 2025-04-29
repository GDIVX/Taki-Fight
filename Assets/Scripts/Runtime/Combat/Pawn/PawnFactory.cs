using Assets.Scripts.Runtime.Combat.Tilemap;
using System;
using Unity.Mathematics;
using UnityEngine;

namespace Runtime.Combat.Pawn
{
    public class PawnFactory : MonoBehaviour
    {
        [SerializeField] private GameObject _prefab;

        internal PawnController CreatePawn(PawnData unit, Tile tile)
        {
            var instance = Instantiate(_prefab, Vector3.zero, quaternion.identity);
            PawnController controller;

            controller = instance.AddComponent<PawnController>();

            controller.Init(unit);
            controller.SetPosition(tile);
            return controller;
        }
    }
}