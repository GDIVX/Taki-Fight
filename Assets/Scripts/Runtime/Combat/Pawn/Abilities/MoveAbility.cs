using System;
using Runtime.Combat.Tilemap;
using UnityEngine;
using Utilities;
using Random = UnityEngine.Random;

namespace Runtime.Combat.Pawn.Abilities
{
    [CreateAssetMenu(fileName = "Pawn Movement Ability", menuName = "Pawns/Abilities/Movement")]
    public class MoveAbility : PawnPlayStrategy
    {
        [SerializeField] private MovementDirection _direction;

        public override void Play(PawnController pawn, Action<bool> onComplete)
        {
            var forward = pawn.Owner == PawnOwner.Player ? Vector2Int.right : Vector2Int.left;
            var tilemap = ServiceLocator.Get<TilemapController>();
            var speed = Potency;

            //calculate movement
            var newPosition = CalculateMovementVector(pawn, forward, speed);
            var tile = tilemap.GetTile(newPosition);
            var isTileValid = tile == null || tile.IsOccupied;
            while (isTileValid && speed > 0)
            {
                speed--;
                newPosition = CalculateMovementVector(pawn, forward, speed);
                tile = tilemap.GetTile(newPosition);
            }

            if (speed <= 0)
            {
                onComplete(false);
                return;
            }

            //move to new position
            pawn.MoveToPosition(tile, () =>
            {
                //execute onMove strategies
                pawn.ExecuteStrategies(pawn.Data.OnMoveStrategies);
                onComplete(true);
            });
        }

        private Vector2Int CalculateMovementVector(PawnController pawn, Vector2Int forward, int speed)
        {
            var movement = _direction switch
            {
                MovementDirection.Forward => forward * speed,
                MovementDirection.Backward => -forward * speed,
                MovementDirection.Up => Vector2Int.up * speed,
                MovementDirection.Down => Vector2Int.down * speed,
                MovementDirection.RandomUpOrDown => Random.value > 0.5f
                    ? Vector2Int.up * speed
                    : Vector2Int.down * speed,
                MovementDirection.RandomForwardOrBackward => Random.value > 0.5f ? forward * speed : -forward * speed,
                MovementDirection.Random => InsideUnitTaxicabCircle() * speed,
                _ => throw new ArgumentOutOfRangeException()
            };

            //find a tile that fits, and it is out of bound, try again with less speed
            var newPosition = pawn.TilemapHelper.AnchorTile.Position + movement;
            return newPosition;
        }

        private Vector2Int InsideUnitTaxicabCircle()
        {
            var x = Random.value * 2 - 1;
            var y = Random.value * 2 - 1;

            var intX = Mathf.FloorToInt(x);
            var intY = Mathf.FloorToInt(y);
            return new Vector2Int(intX, intY);
        }

        public override string GetDescription()
        {
            return _direction switch
            {
                MovementDirection.Forward => $"Dash {Potency} tiles forward",
                MovementDirection.Backward => $"Retreat {Potency} tiles backward",
                MovementDirection.Up => $"Move up {Potency} tiles",
                MovementDirection.Down => $"Move down {Potency} tiles",
                MovementDirection.RandomUpOrDown => $"Move up or down {Potency} tiles",
                MovementDirection.RandomForwardOrBackward => $"Move forward or backward {Potency} tiles",
                MovementDirection.Random => $"Move in a random direction {Potency} tiles",
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}